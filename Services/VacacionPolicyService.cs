using System;
using System.Threading.Tasks;
using SGA_Smash.Repositories;

namespace SGA_Smash.Services
{
    public class VacacionPolicyService : IVacacionPolicyService
    {
        private readonly IVacacionRepository _vacacionRepository;
        private readonly IEmpleadoRepository _empleadoRepository;
        private const int DiasPorAnio = 15; // Política de vacaciones

        public VacacionPolicyService(IVacacionRepository vacacionRepository, IEmpleadoRepository empleadoRepository)
        {
            _vacacionRepository = vacacionRepository;
            _empleadoRepository = empleadoRepository;
        }

        public async Task<(bool ok, string? error, int dias)> ValidarSolicitudAsync(int empleadoId, DateTime inicio, DateTime fin)
        {
            // Validar que el empleado existe
            if (!await _empleadoRepository.EmpleadoExistsAsync(empleadoId))
            {
                return (false, "Empleado no encontrado", 0);
            }

            // Validar fechas
            if (inicio >= fin)
            {
                return (false, "La fecha de inicio debe ser menor a la fecha de fin", 0);
            }

            if (inicio < DateTime.Now.Date)
            {
                return (false, "No se pueden solicitar vacaciones en fechas pasadas", 0);
            }

            // Validar que no haya superposición
            if (await _vacacionRepository.HasOverlapAsync(empleadoId, inicio, fin))
            {
                return (false, "Ya existe una solicitud de vacaciones en este período", 0);
            }

            // Calcular días solicitados
            int diasSolicitados = (int)(fin - inicio).TotalDays;

            // Obtener días disponibles
            int diasDisponibles = await _empleadoRepository.GetDiasDisponiblesAsync(empleadoId);

            if (diasSolicitados > diasDisponibles)
            {
                return (false, $"No tiene suficientes días de vacaciones disponibles. Tiene {diasDisponibles} días", 0);
            }

            return (true, null, diasSolicitados);
        }

        public async Task AplicarAprobacionAsync(int vacacionId)
        {
            var vacacion = await _vacacionRepository.GetByIdAsync(vacacionId);
            if (vacacion == null)
                return;

            // Calcular días utilizados
            int diasUtilizados = (int)(vacacion.FechaFin - vacacion.FechaInicio).TotalDays;

            // Restar de los días disponibles
            int diasActuales = await _empleadoRepository.GetDiasDisponiblesAsync(vacacion.EmpleadoId);
            int nuevosDias = diasActuales - diasUtilizados;

            await _empleadoRepository.SetDiasDisponiblesAsync(vacacion.EmpleadoId, nuevosDias < 0 ? 0 : nuevosDias);
        }
    }
}
