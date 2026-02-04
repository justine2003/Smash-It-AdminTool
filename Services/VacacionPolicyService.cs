using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Services
{
    public class VacacionPolicyService : IVacacionPolicyService
    {
        private readonly IVacacionRepository _repo;
        private readonly IEmpleadoRepository _empleados;

        public VacacionPolicyService(IVacacionRepository repo, IEmpleadoRepository empleados)
        {
            _repo = repo;
            _empleados = empleados;
        }

        public async Task<VacacionValidationResult> ValidarSolicitudAsync(Vacacion v)
        {
            var hoy = DateTime.Today;

            if (v.FechaInicio.Date < hoy)
                return new VacacionValidationResult(false, "No se permiten fechas de inicio en el pasado.");

            if (v.FechaFin.Date < v.FechaInicio.Date)
                return new VacacionValidationResult(false, "La fecha fin no puede ser anterior a la fecha inicio.");

            var dias = (int)(v.FechaFin.Date - v.FechaInicio.Date).TotalDays + 1;
            if (dias <= 0)
                return new VacacionValidationResult(false, "Los días solicitados deben ser mayores a cero.");

            var disponibles = await _empleados.GetDiasDisponiblesAsync(v.EmpleadoId);
            if (dias > disponibles)
                return new VacacionValidationResult(false, $"No hay suficientes días. Disponibles: {disponibles}, solicitados: {dias}.");

            var solapa = await _repo.HasOverlapAsync(v.EmpleadoId, v.FechaInicio.Date, v.FechaFin.Date);
            if (solapa)
                return new VacacionValidationResult(false, "El período se solapa con otra solicitud pendiente o aprobada.");

            return new VacacionValidationResult(true);
        }

        public async Task AplicarAprobacionAsync(Vacacion v)
        {
            var disp = await _empleados.GetDiasDisponiblesAsync(v.EmpleadoId);
            var nuevos = disp - v.DiasSolicitados;
            if (nuevos < 0) nuevos = 0;
            await _empleados.SetDiasDisponiblesAsync(v.EmpleadoId, nuevos);
        }
    }
}
