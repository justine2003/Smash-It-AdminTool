using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using SGA_Smash.Services;
using System;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    // Solo empleados autenticados
    //[Authorize(Roles = "Empleado")]
    //[AllowAnonymous]
    public class VacacionWorkflowController : Controller
    {
        private readonly IVacacionRepository _vacaciones;
        private readonly IVacacionPolicyService _policy;
        private readonly IEmpleadoRepository _empleados;

        public VacacionWorkflowController(
            IVacacionRepository vacaciones,
            IVacacionPolicyService policy,
            IEmpleadoRepository empleados)
        {
            _vacaciones = vacaciones;
            _policy = policy;
            _empleados = empleados;
        }

        private int? GetEmpleadoId()
        {

            var emp = User.FindFirst("EmpleadoId")?.Value;

            if (string.IsNullOrEmpty(emp))
                emp = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(emp, out var id))
                return id;

            return null;
        }

        public async Task<IActionResult> MisSolicitudes()
        {
            var empleadoId = GetEmpleadoId();
            if (empleadoId is null) return RedirectToAction("Login", "Account"); 

            var items = await _vacaciones.GetByEmpleadoAsync(empleadoId.Value);
            ViewBag.EmpleadoId = empleadoId.Value;
            return View("~/Views/Vacacion/MisSolicitudes.cshtml", items);
        }

        [HttpGet]
        public async Task<IActionResult> Solicitar()
        {
            var empleadoId = GetEmpleadoId();
            if (empleadoId is null) return RedirectToAction("Login", "Account");

            var disp = await _empleados.GetDiasDisponiblesAsync(empleadoId.Value);
            ViewBag.Disponibles = disp;

            var m = new Vacacion
            {
                EmpleadoId = empleadoId.Value,
                FechaInicio = DateTime.Today.AddDays(1),
                FechaFin = DateTime.Today.AddDays(1),
                DiasSolicitados = 1
            };
            return View("~/Views/Vacacion/Solicitar.cshtml", m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(Vacacion v)
        {
            var empleadoId = GetEmpleadoId();
            if (empleadoId is null) return RedirectToAction("Login", "Account");

            v.EmpleadoId = empleadoId.Value;

            v.Estado = "Pendiente";
            v.FechaSolicitud = DateTime.Today;
            v.DiasSolicitados = (int)(v.FechaFin.Date - v.FechaInicio.Date).TotalDays + 1;

            var check = await _policy.ValidarSolicitudAsync(v);
            if (!check.Ok)
            {
                var disp = await _empleados.GetDiasDisponiblesAsync(v.EmpleadoId);
                ViewBag.Disponibles = disp;
                ModelState.AddModelError(string.Empty, check.Mensaje ?? "No se pudo validar la solicitud.");
                return View("~/Views/Vacacion/Solicitar.cshtml", v);
            }

            await _vacaciones.AddAsync(v);
            TempData["Success"] = "Solicitud enviada y pendiente de aprobación.";
            return RedirectToAction(nameof(MisSolicitudes));
        }
    }
}
