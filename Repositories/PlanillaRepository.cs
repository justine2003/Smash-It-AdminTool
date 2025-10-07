using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using SGA_Smash.Models;

=======
using SGA_Smash.Data;
using SGA_Smash.Models;


>>>>>>> main
namespace SGA_Smash.Repositories
{
    public class PlanillaRepository : IPlanillaRepository
    {
<<<<<<< HEAD
        private readonly DbContext _context;

        public PlanillaRepository(DbContext context)
=======
        private readonly ApplicationDbContext _context;

        public PlanillaRepository(ApplicationDbContext context)
>>>>>>> main
        {
            _context = context;
        }

        public async Task<IEnumerable<Planilla>> GetAllAsync()
        {
<<<<<<< HEAD
            return await _context.Set<Planilla>()
                .Include(p => p.Empleado) // si tienes navegación
                .AsNoTracking()
                .OrderByDescending(p => p.Anio).ThenByDescending(p => p.Mes)
                .ThenBy(p => p.EmpleadoId)
=======
            return await _context.Planillas
                .AsNoTracking()
>>>>>>> main
                .ToListAsync();
        }

        public async Task<Planilla?> GetByIdAsync(int id)
        {
<<<<<<< HEAD
            return await _context.Set<Planilla>()
                .Include(p => p.Empleado)
=======
            return await _context.Planillas
>>>>>>> main
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Planilla planilla)
        {
<<<<<<< HEAD
            await _context.Set<Planilla>().AddAsync(planilla);
=======
            _context.Planillas.Add(planilla);
>>>>>>> main
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Planilla planilla)
        {
            _context.Entry(planilla).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
<<<<<<< HEAD
            var entity = await _context.Set<Planilla>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Planilla>().Remove(entity);
=======
            var entity = await _context.Planillas.FindAsync(id);
            if (entity != null)
            {
                _context.Planillas.Remove(entity);
>>>>>>> main
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
<<<<<<< HEAD
            return await _context.Set<Planilla>().AnyAsync(e => e.Id == id);
=======
            return await _context.Planillas.AnyAsync(e => e.Id == id);
>>>>>>> main
        }
    }
}
