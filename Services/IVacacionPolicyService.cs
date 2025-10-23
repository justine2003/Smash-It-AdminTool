using System;
using System.Threading.Tasks;

namespace SGA_Smash.Services
{
    public interface IVacacionPolicyService
    {
        Task<(bool ok, string? error, int dias)> ValidarSolicitudAsync(int empleadoId, DateTime inicio, DateTime fin);
        Task AplicarAprobacionAsync(int vacacionId);
    }
}
