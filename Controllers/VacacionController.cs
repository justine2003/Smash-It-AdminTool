using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using SGA_Smash.Services;
using System;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    //[Authorize]
    public class VacacionController : Controller
    {
        private readonly IVacacionRepository _vacaciones;
        private readonly IVacacionPolicyService _policy;

        public VacacionController(IVacacionRepository vacaciones,
                                  IVacacionPolicyService policy)
        {
            _vacaciones = vacaciones;
            _policy = policy;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _vacaciones.GetAllAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _vacaciones.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create(int? empleadoId = null)
        {
            return View(new Vacacion {
                EmpleadoId = empleadoId ?? 0,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today,
                Estado = "Pendiente"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vacacion model)
        {
            model.DiasSolicitados = model.DiasCalculados;
            model.Estado = "Pendiente";
            model.FechaSolicitud = DateTime.Today;

            if (!ModelState.IsValid)
                return View(model);

            var (ok, error, _) = await _policy.ValidarSolicitudAsync(model.EmpleadoId, model.FechaInicio, model.FechaFin);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo validar la solicitud.");
                return View(model);
            }

            await _vacaciones.AddAsync(model);
            TempData["Success"] = "Solicitud de vacaciones registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _vacaciones.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (item.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden editar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vacacion model)
        {
            if (id != model.Id) return NotFound();

            model.DiasSolicitados = model.DiasCalculados;

            if (!ModelState.IsValid)
                return View(model);

            var (ok, error, _) = await _policy.ValidarSolicitudAsync(model.EmpleadoId, model.FechaInicio, model.FechaFin);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo validar la solicitud.");
                return View(model);
            }

            try
            {
                await _vacaciones.UpdateAsync(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _vacaciones.ExistsAsync(model.Id)) return NotFound();
                else throw;
            }

            TempData["Success"] = "Solicitud de vacaciones actualizada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _vacaciones.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vacaciones.DeleteAsync(id);
            TempData["Success"] = "Solicitud de vacaciones eliminada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        // ------ Aprobación (solo Admin) ------

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int id)
        {
            var v = await _vacaciones.GetByIdAsync(id);
            if (v == null) return NotFound();
            if (v.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden aprobar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            var aprobadorId = 0; // si luego mapeas usuario->empleado, coloca su Id
            await _vacaciones.ApproveAsync(v, aprobadorId);
            await _policy.AplicarAprobacionAsync(v.Id);

            TempData["Success"] = "Solicitud aprobada y días descontados.";
            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id)
        {
            var v = await _vacaciones.GetByIdAsync(id);
            if (v == null) return NotFound();
            if (v.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden rechazar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            var aprobadorId = 0;
            await _vacaciones.RejectAsync(v, aprobadorId);

            TempData["Success"] = "Solicitud rechazada.";
            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPendientes()
        {
            var items = await _vacaciones.GetPendientesAsync();
            return View(items); // Views/Vacacion/AdminPendientes.cshtml
        }

        //[Authorize] // cualquier user logueado
        public async Task<IActionResult> MisSolicitudes(int empleadoId)
        {
            var items = await _vacaciones.GetByEmpleadoAsync(empleadoId);
            ViewBag.EmpleadoId = empleadoId;
            return View(items); // Views/Vacacion/MisSolicitudes.cshtml
        }

        // GET: /Vacacion/Solicitar?empleadoId=123
        //[Authorize]
        [HttpGet]
        public IActionResult Solicitar(int empleadoId)
        {
            // Deja el formulario listo con valores iniciales
            var vm = new Vacacion
            {
                EmpleadoId = empleadoId,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today,
                Estado = "Pendiente"
            };

            // Si usas el cálculo en el front para mostrar días, esto es opcional
            vm.DiasSolicitados = vm.DiasCalculados;

            return View(vm); // Views/Vacacion/Solicitar.cshtml
        }

        // POST: /Vacacion/Solicitar
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(Vacacion model)
        {
            // Fuerza los valores que no deben venir del cliente
            model.DiasSolicitados = model.DiasCalculados;
            model.Estado = "Pendiente";
            model.FechaSolicitud = DateTime.Today;

            // 1) Validaciones por DataAnnotations + IValidatableObject
            if (!ModelState.IsValid)
                return View(model);

            // 2) Reglas de negocio (días disponibles + no superposición)
            var (ok, error, _) = await _policy.ValidarSolicitudAsync(model.EmpleadoId, model.FechaInicio, model.FechaFin);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo validar la solicitud de vacaciones.");
                return View(model);
            }

            // 3) Guardar
            await _vacaciones.AddAsync(model);

            // 4) Confirmación y redirección a "MisSolicitudes"
            TempData["Success"] = "Solicitud enviada y marcada como Pendiente.";
            return RedirectToAction(nameof(MisSolicitudes), new { empleadoId = model.EmpleadoId });
        }
    }
}
