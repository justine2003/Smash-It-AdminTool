using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IVacacionRepository
    {
        Task<IEnumerable<Vacacion>> GetAllAsync();
        Task<IEnumerable<Vacacion>> GetByEmpleadoAsync(int empleadoId);
        Task<IEnumerable<Vacacion>> GetPendientesAsync();
        Task<Vacacion?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task AddAsync(Vacacion v);
        Task UpdateAsync(Vacacion v);
        Task DeleteAsync(int id);

        // ⚠️ La interfaz te exige esta firma (3 parámetros)
        Task<bool> HasOverlapAsync(int empleadoId, DateTime inicio, DateTime fin);

        // Acciones de workflow
        Task ApproveAsync(Vacacion v, int aprobadorId);
        Task RejectAsync(Vacacion v, int aprobadorId);
    }
}
