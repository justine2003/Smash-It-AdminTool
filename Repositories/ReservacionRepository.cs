using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class ReservacionRepository : IReservacionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservacion>> GetAllReservaciones()
        {
            return await _context.Reservaciones
                .Include(r => r.Cliente)
                .ToListAsync();
        }

        public async Task<Reservacion> GetReservacionById(int id)
        {
            return await _context.Reservaciones
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddReservacion(Reservacion reservacion)
        {
            _context.Reservaciones.Add(reservacion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReservacion(Reservacion reservacion)
        {
            _context.Entry(reservacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservacion(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion != null)
            {
                _context.Reservaciones.Remove(reservacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ReservacionExists(int id)
        {
            return await _context.Reservaciones.AnyAsync(e => e.Id == id);
        }

        // ============================================
        //        FILTRAR POR ESTADO (Confirmada / Cancelada / Pendiente)
        // ============================================
        public async Task<IEnumerable<Reservacion>> GetReservacionesByEstado(
            string estado,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            var query = _context.Reservaciones
                .Include(r => r.Cliente)
                .Where(r => r.Estado == estado);

            if (desde.HasValue)
                query = query.Where(r => r.FechaHora >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(r => r.FechaHora <= hasta.Value);

            return await query.ToListAsync();
        }

        // ============================================
        //        CONFIRMADAS + CANCELADAS (Para PDF)
        // ============================================
        public async Task<IEnumerable<Reservacion>> GetReservacionesConfirmadasYCanceladas(
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            var query = _context.Reservaciones
                .Include(r => r.Cliente)
                .Where(r => r.Estado == "Confirmada" || r.Estado == "Cancelada");

            if (desde.HasValue)
                query = query.Where(r => r.FechaHora >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(r => r.FechaHora <= hasta.Value);

            return await query.ToListAsync();
        }

        // ============================================
        //        TOP HORARIOS CON MÁS DEMANDA
        // ============================================
        public async Task<IEnumerable<HorarioDemanda>> GetHorariosConMasDemanda(
            DateTime? desde = null,
            DateTime? hasta = null,
            int top = 10)
        {
            var query = _context.Reservaciones
                .Where(r => r.Estado == "Confirmada");

            if (desde.HasValue)
                query = query.Where(r => r.FechaHora >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(r => r.FechaHora <= hasta.Value);

            var result = await query
                .GroupBy(r =>
                    new DateTime(
                        r.FechaHora.Year,
                        r.FechaHora.Month,
                        r.FechaHora.Day,
                        r.FechaHora.Hour,
                        0, 0
                    )
                )
                .Select(g => new HorarioDemanda
                {
                    Hora = g.Key.Hour,
                    CantidadReservaciones = g.Count()
                })
                .OrderByDescending(x => x.CantidadReservaciones)
                .Take(top)
                .ToListAsync();

            return result;
        }

        Task<IEnumerable<Reservacion>> IReservacionRepository.GetAllReservaciones()
        {
            throw new NotImplementedException();
        }

        Task<Reservacion> IReservacionRepository.GetReservacionById(int id)
        {
            throw new NotImplementedException();
        }

        Task IReservacionRepository.AddReservacion(Reservacion reservacion)
        {
            throw new NotImplementedException();
        }

        Task IReservacionRepository.UpdateReservacion(Reservacion reservacion)
        {
            throw new NotImplementedException();
        }

        Task IReservacionRepository.DeleteReservacion(int id)
        {
            throw new NotImplementedException();
        }

        Task<bool> IReservacionRepository.ReservacionExists(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<HorarioDemanda>> IReservacionRepository.GetHorariosConMasDemanda(DateTime? desde, DateTime? hasta, int top)
        {
            return GetHorariosConMasDemanda(desde, hasta, top);
        }

        Task<IEnumerable<Reservacion>> IReservacionRepository.GetReservacionesConfirmadasYCanceladas(DateTime? desde, DateTime? hasta)
        {
            return GetReservacionesConfirmadasYCanceladas(desde, hasta);
        }

        // ============================================
        //        OBTENER MESAS DISPONIBLES
        // ============================================
        public async Task<List<MesaDisponible>> GetMesasDisponibles(DateTime fechaHora, int numeroPersonas, int? reservacionIdExcluir = null)
        {
            // Normalizar la fecha/hora para comparar solo año, mes, día y hora (sin minutos/segundos)
            var fechaHoraInicio = new DateTime(fechaHora.Year, fechaHora.Month, fechaHora.Day, fechaHora.Hour, 0, 0);
            var fechaHoraFin = fechaHoraInicio.AddHours(1);

            // Obtener las mesas ocupadas para la fecha y hora especificada
            // Consideramos ocupadas las mesas con reservaciones confirmadas o pendientes (no canceladas)
            // que se solapen con el rango de tiempo solicitado
            var queryReservaciones = _context.Reservaciones
                .Where(r => r.FechaHora >= fechaHoraInicio &&
                           r.FechaHora < fechaHoraFin &&
                           r.Estado != "Cancelada");

            // Si se está editando una reservación, excluirla de la lista de ocupadas
            if (reservacionIdExcluir.HasValue)
            {
                queryReservaciones = queryReservaciones.Where(r => r.Id != reservacionIdExcluir.Value);
            }

            var mesasOcupadas = await queryReservaciones
                .Select(r => r.Mesa)
                .Distinct()
                .ToListAsync();

            // Obtener todas las mesas desde la BD que tengan capacidad suficiente
            var mesasDisponibles = await _context.Mesas
                .Where(m => m.Capacidad >= numeroPersonas && // Solo mesas con capacidad suficiente
                           !mesasOcupadas.Contains(m.Numero)) // Excluir mesas ocupadas
                .Select(m => new MesaDisponible
                {
                    Numero = m.Numero,
                    Capacidad = m.Capacidad
                })
                .OrderBy(m => m.Capacidad) // Ordenar por capacidad (menor a mayor)
                .ThenBy(m => m.Numero) // Luego por número
                .ToListAsync();

            return mesasDisponibles;
        }

        Task<List<MesaDisponible>> IReservacionRepository.GetMesasDisponibles(DateTime fechaHora, int numeroPersonas, int? reservacionIdExcluir)
        {
            return GetMesasDisponibles(fechaHora, numeroPersonas, reservacionIdExcluir);
        }
    }
}
