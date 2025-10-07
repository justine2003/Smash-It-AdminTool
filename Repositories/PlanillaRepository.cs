
using SGA_Smash.Data;
using SGA_Smash.Models;


namespace SGA_Smash.Repositories
{
    public class PlanillaRepository : IPlanillaRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanillaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Planilla>> GetAllAsync()
        {
            return await _context.Planillas
                .AsNoTracking()
                .ThenByDescending(p => p.Mes)
                .ThenBy(p => p.EmpleadoId)
                .ToListAsync();
        }

        public async Task<Planilla?> GetByIdAsync(int id)
        {
            return await _context.Planillas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Planilla planilla)
        {
            _context.Planillas.Add(planilla);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Planilla planilla)
        {
            _context.Entry(planilla).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Planillas.FindAsync(id);
            if (entity != null)
            {
                _context.Planillas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Planillas.AnyAsync(e => e.Id == id);
        }
    }
}
