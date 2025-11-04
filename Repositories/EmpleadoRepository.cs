using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly ApplicationDbContext _db;

        public EmpleadoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Si usas estos dos en otros lugares, los dejo:
        public async Task<IEnumerable<Empleado>> GetAllAsync() =>
            await _db.Empleados
                     .AsNoTracking()
                     .Where(e => e.Activo)
                     .OrderBy(e => e.Nombre)
                     .ToListAsync();

        public Task<Empleado?> GetByIdAsync(int id) =>
            _db.Empleados
               .AsNoTracking()
               .FirstOrDefaultAsync(e => e.Id == id);

        // Métodos solicitados por tu controlador actual
        public async Task<IEnumerable<Empleado>> GetAllEmpleadosAsync() =>
            await _db.Empleados
                     .AsNoTracking()
                     .OrderBy(e => e.Nombre)
                     .ToListAsync();

        public async Task<Empleado> GetEmpleadoByIdAsync(int id)
        {
            // Si quieres no-tracking:
            var emp = await _db.Empleados
                               .AsNoTracking()
                               .FirstOrDefaultAsync(e => e.Id == id);
            return emp!;
        }

        public async Task AddEmpleadoAsync(Empleado empleado)
        {
            _db.Empleados.Add(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateEmpleadoAsync(Empleado empleado)
        {
            _db.Entry(empleado).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEmpleadoAsync(int id)
        {
            var empleado = await _db.Empleados.FindAsync(id);
            if (empleado is null) return;

            _db.Empleados.Remove(empleado);
            await _db.SaveChangesAsync();
        }

        public Task<bool> EmpleadoExistsAsync(int id) =>
            _db.Empleados.AnyAsync(e => e.Id == id);

        // Soporte para vacaciones
        public async Task<int> GetDiasDisponiblesAsync(int empleadoId)
        {
            var dias = await _db.Empleados
                                .AsNoTracking()
                                .Where(e => e.Id == empleadoId)
                                .Select(e => e.DiasVacacionesDisponibles)
                                .FirstOrDefaultAsync();
            return dias; // 0 si no existe
        }

        public async Task SetDiasDisponiblesAsync(int empleadoId, int nuevosDias)
        {
            var empleado = await _db.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);
            if (empleado is null) return;

            empleado.DiasVacacionesDisponibles = nuevosDias < 0 ? 0 : nuevosDias;
            await _db.SaveChangesAsync();
        }
    }
}
