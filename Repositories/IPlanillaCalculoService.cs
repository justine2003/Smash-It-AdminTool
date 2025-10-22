
namespace SGA_Smash.Repositories
{
    public interface IPlanillaCalculoService
    {
        Task<(decimal deducciones, decimal bonificaciones)> CalcularConceptosActivosAsync(int empleadoId);
    }
}
