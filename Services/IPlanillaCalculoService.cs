
namespace SGA_Smash.Services
{
    public interface IPlanillaCalculoService
    {
        Task<(decimal deducciones, decimal bonificaciones)> CalcularConceptosActivosAsync(int empleadoId);
    }
}
