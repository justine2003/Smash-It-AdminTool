using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Empleado>> GetAllEmpleadosAsync()
        {
            return await _context.Empleados.ToListAsync();
        }

        public async Task<Empleado> GetEmpleadoByIdAsync(int id)
        {
            return await _context.Empleados.FindAsync(id);
        }

        public async Task AddEmpleadoAsync(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmpleadoAsync(Empleado empleado)
        {
            _context.Entry(empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmpleadoAsync(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmpleadoExistsAsync(int id)
        {
            return await _context.Empleados.AnyAsync(e => e.Id == id);
        }

        public async Task<int> GetDiasDisponiblesAsync(int empleadoId)
        {
            var empleado = await _context.Set<Empleado>()
                .AsNoTracking()
                .Where(e => e.Id == empleadoId)
                .Select(e => e.DiasVacacionesDisponibles)
                .FirstOrDefaultAsync();

            return empleado; // si no existe, devolverá 0
        }

        public async Task SetDiasDisponiblesAsync(int empleadoId, int nuevosDias)
        {
            var empleado = await _context.Set<Empleado>().FirstOrDefaultAsync(e => e.Id == empleadoId);
            if (empleado == null) return;

            empleado.DiasVacacionesDisponibles = nuevosDias < 0 ? 0 : nuevosDias;
            await _context.SaveChangesAsync();
        }
    }
}