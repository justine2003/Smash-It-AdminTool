using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class PlanillaRepository : IPlanillaRepository
    {
        private readonly ApplicationDbContext _db;
        public PlanillaRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Planilla>> GetAllAsync() =>
            await _db.Planillas.AsNoTracking()
                .Include(p => p.Empleado)
                .OrderByDescending(p => p.Anio).ThenByDescending(p => p.Mes)
                .ToListAsync();

        public async Task<IEnumerable<Planilla>> GetByMesAnioAsync(int mes, int anio) =>
            await _db.Planillas.AsNoTracking()
                .Include(p => p.Empleado)
                .Where(p => p.Mes == mes && p.Anio == anio)
                .OrderBy(p => p.Empleado!.Nombre)
                .ToListAsync();

        public Task<Planilla?> GetByIdAsync(int id) =>
            _db.Planillas.Include(p => p.Empleado).FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Planilla p)
        {
            await _db.Planillas.AddAsync(p);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Planilla p)
        {
            _db.Entry(p).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _db.Planillas.FindAsync(id);
            if (e != null)
            {
                _db.Planillas.Remove(e);
                await _db.SaveChangesAsync();
            }
        }

        public Task<bool> ExistsPeriodoAsync(int empleadoId, int mes, int anio) =>
            _db.Planillas.AnyAsync(p => p.EmpleadoId == empleadoId && p.Mes == mes && p.Anio == anio);
    }
}
