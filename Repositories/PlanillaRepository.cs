using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class PlanillaRepository : IPlanillaRepository
    {
        private readonly DbContext _context;

        public PlanillaRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Planilla>> GetAllAsync()
        {
            return await _context.Set<Planilla>()
                .Include(p => p.Empleado) // si tienes navegación
                .AsNoTracking()
                .OrderByDescending(p => p.Anio).ThenByDescending(p => p.Mes)
                .ThenBy(p => p.EmpleadoId)
                .ToListAsync();
        }

        public async Task<Planilla?> GetByIdAsync(int id)
        {
            return await _context.Set<Planilla>()
                .Include(p => p.Empleado)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Planilla planilla)
        {
            await _context.Set<Planilla>().AddAsync(planilla);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Planilla planilla)
        {
            _context.Entry(planilla).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Planilla>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Planilla>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<Planilla>().AnyAsync(e => e.Id == id);
        }
    }
}
