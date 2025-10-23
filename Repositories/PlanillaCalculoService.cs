using SGA_Smash.Repositories;

namespace SGA_Smash.Repositories
{
    public class PlanillaCalculoService : IPlanillaCalculoService
    {
        private readonly IConceptoNominaRepository _conceptos;
        public PlanillaCalculoService(IConceptoNominaRepository conceptos) => _conceptos = conceptos;

        public async Task<(decimal deducciones, decimal bonificaciones)> CalcularConceptosActivosAsync(int empleadoId)
        {
            var (ded, bon) = await _conceptos.GetTotalesActivosAsync(empleadoId);
            return (ded, bon);
        }
    }
}
