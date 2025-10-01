using System.Collections.Generic;
using System.Threading.Tasks;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IPlanillaRepository
    {
        Task<IEnumerable<Planilla>> GetAllAsync();
        Task<Planilla?> GetByIdAsync(int id);
        Task AddAsync(Planilla planilla);
        Task UpdateAsync(Planilla planilla);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
