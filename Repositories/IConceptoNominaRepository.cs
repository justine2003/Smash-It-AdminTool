using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IConceptoNominaRepository
    {
        Task<(decimal deducciones, decimal bonificaciones)> GetTotalesActivosAsync(int empleadoId);
        Task<IEnumerable<ConceptoNomina>> GetByEmpleadoAsync(int empleadoId);
        Task<ConceptoNomina?> GetByIdAsync(int id);
        Task CreateAsync(ConceptoNomina c, string usuario);
        Task UpdateAsync(ConceptoNomina c, string usuario, ConceptoNomina before);
        Task DeleteAsync(int id, string usuario);
        Task ToggleActivoAsync(int id, string usuario);
    }
}
