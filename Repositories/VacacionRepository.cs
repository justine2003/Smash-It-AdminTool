using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA_Smash.Repositories
{
    public class VacacionRepository : IVacacionRepository
    {
        private readonly ApplicationDbContext _db;
        public VacacionRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Vacacion>> GetAllAsync() =>
            await _db.Set<Vacacion>()
                     .AsNoTracking()
                     .Include(v => v.Empleado)
                     .OrderByDescending(v => v.FechaSolicitud)
                     .ToListAsync();

        public Task<Vacacion?> GetByIdAsync(int id) =>
            _db.Set<Vacacion>()
               .Include(v => v.Empleado)
               .FirstOrDefaultAsync(v => v.Id == id);

        public async Task AddAsync(Vacacion v)
        {
            await _db.Set<Vacacion>().AddAsync(v);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vacacion v)
        {
            _db.Entry(v).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(int id) =>
            _db.Set<Vacacion>().AnyAsync(e => e.Id == id);

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Set<Vacacion>().FindAsync(id);
            if (entity != null)
            {
                _db.Set<Vacacion>().Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Vacacion>> GetByEmpleadoAsync(int empleadoId) =>
            await _db.Set<Vacacion>()
                     .AsNoTracking()
                     .Where(v => v.EmpleadoId == empleadoId)
                     .OrderByDescending(v => v.FechaSolicitud)
                     .ToListAsync();

        public async Task<IEnumerable<Vacacion>> GetPendientesAsync() =>
            await _db.Set<Vacacion>()
                     .AsNoTracking()
                     .Where(v => v.Estado == "Pendiente")
                     .Include(v => v.Empleado)
                     .OrderBy(v => v.FechaInicio)
                     .ToListAsync();

        public async Task<bool> HasOverlapAsync(int empleadoId, DateTime inicio, DateTime fin) =>
            await _db.Set<Vacacion>()
                     .AsNoTracking()
                     .AnyAsync(v => v.EmpleadoId == empleadoId
                                    && v.Estado != "Rechazada"
                                    && v.FechaInicio <= fin
                                    && v.FechaFin >= inicio);

        public async Task ApproveAsync(Vacacion v, int aprobadorId)
        {
            v.Estado = "Aprobada";
            v.AprobadoPor = aprobadorId;
            _db.Entry(v).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(Vacacion v, int aprobadorId)
        {
            v.Estado = "Rechazada";
            v.AprobadoPor = aprobadorId;
            _db.Entry(v).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
