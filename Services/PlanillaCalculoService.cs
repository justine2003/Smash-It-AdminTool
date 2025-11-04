using SGA_Smash.Repositories;

namespace SGA_Smash.Services
{
    public class PlanillaCalculoService
    {
        private readonly IConceptoNominaRepository _conceptos;
        public PlanillaCalculoService(IConceptoNominaRepository conceptos) => _conceptos = conceptos;

        public Task<(decimal deducciones, decimal bonificaciones)> CalcularActivosAsync(int empleadoId) =>
            _conceptos.GetTotalesActivosAsync(empleadoId);
    }
}
