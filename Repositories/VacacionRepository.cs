using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class VacacionRepository : IVacacionRepository
    {
        private readonly ApplicationDbContext _context;

        public VacacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vacacion>> GetAllAsync()
        {
            return await _context.Set<Vacacion>()
                .Include(v => v.Empleado)
                .Include(v => v.Aprobador)
                .AsNoTracking()
                .OrderByDescending(v => v.FechaSolicitud)
                .ThenBy(v => v.EmpleadoId)
                .ToListAsync();
        }

        public async Task<Vacacion?> GetByIdAsync(int id)
        {
            return await _context.Set<Vacacion>()
                .Include(v => v.Empleado)
                .Include(v => v.Aprobador)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task AddAsync(Vacacion vacacion)
        {
            if (vacacion.DiasSolicitados <= 0)
                vacacion.DiasSolicitados = (int)(vacacion.FechaFin.Date - vacacion.FechaInicio.Date).TotalDays + 1;

            if (vacacion.FechaSolicitud == default)
                vacacion.FechaSolicitud = System.DateTime.Today;

            await _context.Set<Vacacion>().AddAsync(vacacion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vacacion vacacion)
        {
            if (vacacion.DiasSolicitados <= 0)
                vacacion.DiasSolicitados = (int)(vacacion.FechaFin.Date - vacacion.FechaInicio.Date).TotalDays + 1;

            _context.Entry(vacacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<Vacacion>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Vacacion>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<Vacacion>().AnyAsync(e => e.Id == id);
        }
    }
}
