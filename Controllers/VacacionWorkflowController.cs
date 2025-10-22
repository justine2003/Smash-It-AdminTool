using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using SGA_Smash.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    [Authorize] // cualquier usuario autenticado puede solicitar/mis solicitudes
    public class VacacionWorkflowController : Controller
    {
        private readonly IVacacionRepository _repo;
        private readonly IEmpleadoRepository _empleados;
        private readonly IVacacionPolicyService _policy;
        private readonly INotificationService _notify;

        public VacacionWorkflowController(
            IVacacionRepository repo,
            IEmpleadoRepository empleados,
            IVacacionPolicyService policy,
            INotificationService notify)
        {
            _repo = repo;
            _empleados = empleados;
            _policy = policy;
            _notify = notify;
        }

        // Empleado: ver mis solicitudes
        public async Task<IActionResult> MisSolicitudes(int empleadoId)
        {
            var items = await _repo.GetByEmpleadoAsync(empleadoId);
            ViewBag.EmpleadoId = empleadoId;
            return View(items);
        }

        // Empleado: solicitar
        public IActionResult Solicitar(int empleadoId)
        {
            ViewBag.EmpleadoId = empleadoId;
            return View(new Vacacion
            {
                EmpleadoId = empleadoId,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today
            });
        }

        // ADMIN: lista de pendientes
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPendientes()
        {
            var items = await _repo.GetPendientesAsync();
            return View(items);
        }

        // ADMIN: aprobar
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int id)
        {
            var v = await _repo.GetByIdAsync(id);
            if (v == null) return NotFound();

            var aprobadorId = GetCurrentEmpleadoIdOrDefault();
            await _repo.ApproveAsync(v, aprobadorId);
            await _policy.AplicarAprobacionAsync(v.Id);

            TempData["Success"] = "Solicitud aprobada y días descontados.";
            return RedirectToAction(nameof(AdminPendientes));
        }

        // ADMIN: rechazar
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id)
        {
            var v = await _repo.GetByIdAsync(id);
            if (v == null) return NotFound();

            var aprobadorId = GetCurrentEmpleadoIdOrDefault();
            await _repo.RejectAsync(v, aprobadorId);

            TempData["Success"] = "Solicitud rechazada.";
            return RedirectToAction(nameof(AdminPendientes));
        }

        private int GetCurrentEmpleadoIdOrDefault()
        {
            // Si manejas relación Usuario->Empleado, retorna el id real; por defecto 0.
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return 0;
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(Vacacion model)
        {
            // 1) Asegurar que DiasSolicitados viene del rango de fechas
            model.DiasSolicitados = model.DiasCalculados;
            ViewBag.EmpleadoId = model.EmpleadoId;

            // 2) Validación de anotaciones + IValidatableObject (tu Vacacion.cs ya valida)
            //    Nota: ModelState ya se pobla con data annotations/IValidatableObject
            if (!ModelState.IsValid)
            {
                // Si quieres, puedes agregar un mensaje general
                if (!ModelState.ContainsKey(string.Empty))
                    ModelState.AddModelError(string.Empty, "Revise los campos marcados.");
                return View(model);
            }

            // 3) Validación de negocio con el servicio (días disponibles y no superposición)
            //    Aunque ya lo llamamos desde IValidatableObject, lo dejamos aquí por si
            //    el servicio no está registrado o deseas doble garantía.
            var (ok, error, _dias) = await _policy.ValidarSolicitudAsync(model.EmpleadoId, model.FechaInicio, model.FechaFin);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo validar la solicitud de vacaciones.");
                return View(model);
            }

            // 4) Seteos finales del registro
            model.Estado = "Pendiente";
            model.FechaSolicitud = DateTime.Today;

            // 5) Persistir
            await _repo.AddAsync(model);

            // 6) Notificar a administradores (stub configurable)
            await _notify.NotifyAdminsAsync(
                "Nueva solicitud de vacaciones",
                $"Empleado #{model.EmpleadoId} solicitó {model.DiasSolicitados} días: {model.FechaInicio:dd/MM/yyyy} - {model.FechaFin:dd/MM/yyyy}."
            );

            // 7) Confirmación
            TempData["Success"] = "Solicitud enviada y marcada como Pendiente.";
            return RedirectToAction(nameof(MisSolicitudes), new { empleadoId = model.EmpleadoId });
        }

    }
}
