using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConceptoNominaController : Controller
    {
        private readonly IConceptoNominaRepository _repo;
        private readonly IEmpleadoRepository _empleados;

        public ConceptoNominaController(IConceptoNominaRepository repo, IEmpleadoRepository empleados)
        {
            _repo = repo;
            _empleados = empleados;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int empleadoId)
        {
            var emp = await _empleados.GetByIdAsync(empleadoId);
            if (emp == null) return NotFound();
            ViewBag.Empleado = emp;
            var items = await _repo.GetByEmpleadoAsync(empleadoId);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int empleadoId, string tipo, string nombre, decimal monto, bool activo = true)
        {
            var usuario = User?.Identity?.Name ?? "system";
            if (tipo != "Deduccion" && tipo != "Bonificacion")
            {
                TempData["Error"] = "Tipo inválido.";
                return RedirectToAction(nameof(Manage), new { empleadoId });
            }

            await _repo.CreateAsync(new ConceptoNomina
            {
                EmpleadoId = empleadoId,
                Tipo = tipo,
                Nombre = nombre,
                Monto = monto,
                Activo = activo
            }, usuario);

            TempData["Success"] = "Concepto creado.";
            return RedirectToAction(nameof(Manage), new { empleadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, decimal monto, bool activo)
        {
            var usuario = User?.Identity?.Name ?? "system";
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return NotFound();
            var before = new ConceptoNomina { Id=c.Id, EmpleadoId=c.EmpleadoId, Tipo=c.Tipo, Nombre=c.Nombre, Monto=c.Monto, Activo=c.Activo };
            c.Monto = monto;
            c.Activo = activo;
            await _repo.UpdateAsync(c, usuario, before);
            TempData["Success"] = "Concepto actualizado.";
            return RedirectToAction(nameof(Manage), new { empleadoId = c.EmpleadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var usuario = User?.Identity?.Name ?? "system";
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return NotFound();
            await _repo.ToggleActivoAsync(id, usuario);
            TempData["Success"] = "Estado actualizado.";
            return RedirectToAction(nameof(Manage), new { empleadoId = c.EmpleadoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = User?.Identity?.Name ?? "system";
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return NotFound();
            var empId = c.EmpleadoId;
            await _repo.DeleteAsync(id, usuario);
            TempData["Success"] = "Concepto eliminado.";
            return RedirectToAction(nameof(Manage), new { empleadoId = empId });
        }
    }
}
