using SGA_Smash.Models;

namespace SGA_Smash.Services
{

public record VacacionValidationResult(bool Ok, string? Mensaje = null);

public interface IVacacionPolicyService
{
    Task<VacacionValidationResult> ValidarSolicitudAsync(Vacacion v);
    Task AplicarAprobacionAsync(Vacacion v);
}
}
