using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IPlanillaRepository
    {
        Task<IEnumerable<Planilla>> GetAllAsync();
        Task<IEnumerable<Planilla>> GetByMesAnioAsync(int mes, int anio);
        Task<Planilla?> GetByIdAsync(int id);
        Task AddAsync(Planilla p);
        Task UpdateAsync(Planilla p);
        Task DeleteAsync(int id);
        Task<bool> ExistsPeriodoAsync(int empleadoId, int mes, int anio);
    }
}
