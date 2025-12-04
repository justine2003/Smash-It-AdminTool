using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using SGA_Smash.Services;
using System;
using System.Linq;

namespace SGA_Smash.Controllers
{
    public class VacacionController : Controller
    {
        private readonly IVacacionRepository _vacaciones;
        private readonly IVacacionPolicyService _policy;
        private readonly IEmpleadoRepository _empleadoRepository;

        public VacacionController(
            IVacacionRepository vacaciones,
            IVacacionPolicyService policy,
            IEmpleadoRepository empleadoRepository)
        {
            _vacaciones = vacaciones;
            _policy = policy;
            _empleadoRepository = empleadoRepository;
        }

        // GET: /Vacacion
        [HttpGet]
        public async Task<IActionResult> Index(int? empleadoId, string? estado, DateTime? desde, DateTime? hasta)
        {
            var empleados = await _empleadoRepository.GetAllEmpleadosAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", empleadoId);

            ViewBag.EmpleadoId = empleadoId;
            ViewBag.Estado = estado ?? "";
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;

            var data = await _vacaciones.GetAllAsync(); // ya incluye Empleado
            if (empleadoId.HasValue)
                data = data.Where(v => v.EmpleadoId == empleadoId.Value);
            if (!string.IsNullOrEmpty(estado))
                data = data.Where(v => v.Estado == estado);
            if (desde.HasValue)
                data = data.Where(v => v.FechaSolicitud >= desde.Value.Date);
            if (hasta.HasValue)
                data = data.Where(v => v.FechaSolicitud <= hasta.Value.Date);

            return View(data.ToList());
        }

        // POST de filtros -> redirige a GET Index con querystring
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filtrar(int? empleadoId, string? estado, DateTime? desde, DateTime? hasta)
        {
            return RedirectToAction(nameof(Index), new
            {
                empleadoId = empleadoId,
                estado = estado,
                desde = desde?.ToString("yyyy-MM-dd"),
                hasta = hasta?.ToString("yyyy-MM-dd")
            });
        }

        // GET: /Vacacion/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _vacaciones.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // crear solicitudes desde admin
        [HttpGet]
        public IActionResult Create(int? empleadoId = null)
        {
            return View(new Vacacion
            {
                EmpleadoId = empleadoId ?? 0,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today,
                Estado = "Pendiente"
            });
        }

        // POST: /Vacacion/Aprobar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int id, string? comentario)
        {
            var v = await _vacaciones.GetByIdAsync(id);
            if (v == null) return NotFound();
            if (v.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden aprobar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            v.Estado = "Aprobada";
            v.ComentarioAdmin = string.IsNullOrWhiteSpace(comentario) ? "Aprobada" : comentario;
            await _vacaciones.UpdateAsync(v);           
            await _policy.AplicarAprobacionAsync(v);    

            TempData["Success"] = "Solicitud aprobada.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Vacacion/Rechazar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id, string? comentario)
        {
            var v = await _vacaciones.GetByIdAsync(id);
            if (v == null) return NotFound();
            if (v.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden rechazar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Index));
            }

            v.Estado = "Rechazada";
            v.ComentarioAdmin = string.IsNullOrWhiteSpace(comentario) ? "Rechazada" : comentario;
            await _vacaciones.UpdateAsync(v);

            TempData["Success"] = "Solicitud rechazada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
