using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

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
                     .OrderByDescending(v => v.FechaSolicitud)
                     .ToListAsync();

        public Task<Vacacion?> GetByIdAsync(int id) =>
            _db.Set<Vacacion>()
               .AsNoTracking()
               .Include(v => v.Empleado)
               .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<bool> ExistsAsync(int id) =>
            await _db.Set<Vacacion>()
                     .AsNoTracking()
                     .AnyAsync(v => v.Id == id);

        public async Task AddAsync(Vacacion v)
        {
            _db.Set<Vacacion>().Add(v);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vacacion v)
        {
            _db.Set<Vacacion>().Update(v);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var v = await _db.Set<Vacacion>().FirstOrDefaultAsync(x => x.Id == id);
            if (v is null) return;
            _db.Set<Vacacion>().Remove(v);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(int empleadoId, DateTime inicio, DateTime fin)
        {
            return await _db.Set<Vacacion>()
                            .AsNoTracking()
                            .Where(v => v.EmpleadoId == empleadoId
                                     && v.Estado != "Rechazada"
                                     && v.FechaInicio <= fin
                                     && v.FechaFin >= inicio)
                            .AnyAsync();
        }

        public async Task ApproveAsync(Vacacion v, int aprobadorId)
        {
            _db.Attach(v);
            v.Estado = "Aprobada";
            v.AprobadoPor = aprobadorId;
            if (string.IsNullOrWhiteSpace(v.ComentarioAdmin))
                v.ComentarioAdmin = "Aprobada";

            _db.Entry(v).Property(x => x.Estado).IsModified = true;
            _db.Entry(v).Property(x => x.AprobadoPor).IsModified = true;
            _db.Entry(v).Property(x => x.ComentarioAdmin).IsModified = true;

            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(Vacacion v, int aprobadorId)
        {
            _db.Attach(v);
            v.Estado = "Rechazada";
            v.AprobadoPor = null;
            if (string.IsNullOrWhiteSpace(v.ComentarioAdmin))
                v.ComentarioAdmin = "Rechazada";

            _db.Entry(v).Property(x => x.Estado).IsModified = true;
            _db.Entry(v).Property(x => x.AprobadoPor).IsModified = true;
            _db.Entry(v).Property(x => x.ComentarioAdmin).IsModified = true;

            await _db.SaveChangesAsync();
        }
    }
}
