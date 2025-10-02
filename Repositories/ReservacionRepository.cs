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
            // Incluye Cliente para mostrar el nombre
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
    }
}
