using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ConceptoNominaController : Controller
    {
        private readonly IConceptoNominaRepository _repo;
        public ConceptoNominaController(IConceptoNominaRepository repo) => _repo = repo;

        // Lista conceptos activos por empleado
        public async Task<IActionResult> Index(int empleadoId)
        {
            var items = await _repo.GetActivosByEmpleadoAsync(empleadoId);
            ViewBag.EmpleadoId = empleadoId;
            return View(items);
        }

        // Editar monto (solo activos)
        public async Task<IActionResult> Edit(int id, int empleadoId)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            if (!item.Activo)
            {
                TempData["Error"] = "Solo se pueden modificar conceptos activos.";
                return RedirectToAction(nameof(Index), new { empleadoId });
            }

            ViewBag.EmpleadoId = empleadoId;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int empleadoId, ConceptoNomina model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EmpleadoId = empleadoId;
                return View(model);
            }

            try
            {
                var user = User?.Identity?.Name ?? "sistema";
                await _repo.UpdateAsync(model, user);
                TempData["Success"] = "Concepto actualizado correctamente.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.EmpleadoId = empleadoId;
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { empleadoId });
        }
    }
}
