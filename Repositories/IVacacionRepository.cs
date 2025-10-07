using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IVacacionRepository
    {
        Task<IEnumerable<Vacacion>> GetAllAsync();
        Task<Vacacion?> GetByIdAsync(int id);
        Task AddAsync(Vacacion vacacion);
        Task UpdateAsync(Vacacion vacacion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
