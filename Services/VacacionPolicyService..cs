using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System;
using System.Threading.Tasks;

namespace SGA_Smash.Services
{
    public class VacacionPolicyService : IVacacionPolicyService
    {
        private readonly ApplicationDbContext _db;
        public VacacionPolicyService(ApplicationDbContext db) => _db = db;

        public async Task<(bool ok, string? error, int dias)> ValidarSolicitudAsync(int empleadoId, DateTime inicio, DateTime fin)
        {
            if (fin.Date < inicio.Date) return (false, "La fecha fin no puede ser anterior a la fecha de inicio.", 0);

            var empleado = await _db.Set<Empleado>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == empleadoId);
            if (empleado is null) return (false, "Empleado no encontrado.", 0);

            int dias = (int)(fin.Date - inicio.Date).TotalDays + 1;
            if (dias <= 0) return (false, "El rango de fechas no es válido.", 0);

            bool overlap = await _db.Set<Vacacion>().AsNoTracking()
                .AnyAsync(v => v.EmpleadoId == empleadoId
                               && v.Estado != "Rechazada"
                               && v.FechaInicio <= fin
                               && v.FechaFin >= inicio);
            if (overlap) return (false, "Ya existe una solicitud que se superpone con esas fechas.", 0);

            if (empleado.DiasVacacionesDisponibles < dias)
                return (false, $"No tiene suficientes días disponibles. Disponibles: {empleado.DiasVacacionesDisponibles}, solicitados: {dias}.", dias);

            return (true, null, dias);
        }

        public async Task AplicarAprobacionAsync(int vacacionId)
        {
            var v = await _db.Set<Vacacion>().FirstOrDefaultAsync(x => x.Id == vacacionId);
            if (v == null || v.Estado != "Aprobada") return;

            var e = await _db.Set<Empleado>().FirstOrDefaultAsync(x => x.Id == v.EmpleadoId);
            if (e == null) return;

            e.DiasVacacionesDisponibles = Math.Max(0, e.DiasVacacionesDisponibles - v.DiasSolicitados);
            await _db.SaveChangesAsync();
        }
    }
}
