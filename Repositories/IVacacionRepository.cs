using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IVacacionRepository
    {
        // CRUD
        Task<IEnumerable<Vacacion>> GetAllAsync();
        Task<Vacacion?> GetByIdAsync(int id);
        Task AddAsync(Vacacion v);
        Task UpdateAsync(Vacacion v);
        Task<bool> ExistsAsync(int id);
        Task DeleteAsync(int id);

        // Flujos
        Task<IEnumerable<Vacacion>> GetByEmpleadoAsync(int empleadoId);
        Task<IEnumerable<Vacacion>> GetPendientesAsync();
        Task<bool> HasOverlapAsync(int empleadoId, DateTime inicio, DateTime fin);
        Task ApproveAsync(Vacacion v, int aprobadorId);
        Task RejectAsync(Vacacion v, int aprobadorId);
    }
}
