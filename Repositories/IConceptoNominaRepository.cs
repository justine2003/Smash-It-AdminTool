using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IConceptoNominaRepository
    {
        Task<IEnumerable<ConceptoNomina>> GetActivosByEmpleadoAsync(int empleadoId);
        Task<ConceptoNomina?> GetByIdAsync(int id);
        Task UpdateAsync(ConceptoNomina concepto, string usuario);
        Task<(decimal totalDeducciones, decimal totalBonificaciones)> GetTotalesActivosAsync(int empleadoId);
    }
}
