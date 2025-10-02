using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    public class PlanillaController : Controller
    {
        private readonly IPlanillaRepository _planillaRepository;
        private readonly IEmpleadoRepository _empleadoRepository;

        public PlanillaController(IPlanillaRepository planillaRepository, IEmpleadoRepository empleadoRepository)
        {
            _planillaRepository = planillaRepository;
            _empleadoRepository = empleadoRepository;
        }

        private async Task LoadEmpleadosAsync(int? selectedId = null)
        {
            var empleados = await _empleadoRepository.GetAllEmpleadosAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", selectedId);
        }

        public async Task<IActionResult> Index()
        {
            var planillas = await _planillaRepository.GetAllAsync();
            return View(planillas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var planilla = await _planillaRepository.GetByIdAsync(id);
            if (planilla == null) return NotFound();
            return View(planilla);
        }

        public async Task<IActionResult> Create()
        {
            await LoadEmpleadosAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Planilla planilla)
        {
            if (!ModelState.IsValid)
            {
                await LoadEmpleadosAsync(planilla.EmpleadoId);
                return View(planilla);
            }

            await _planillaRepository.AddAsync(planilla);
            TempData["Success"] = "Planilla registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var planilla = await _planilla_repository_Get(id);
            if (planilla == null) return NotFound();

            await LoadEmpleadosAsync(planilla.EmpleadoId);
            return View(planilla);
        }

        private async Task<Planilla?> _planilla_repository_Get(int id)
        {
            return await _planillaRepository.GetByIdAsync(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Planilla planilla)
        {
            if (id != planilla.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadEmpleadosAsync(planilla.EmpleadoId);
                return View(planilla);
            }

            try
            {
                await _planillaRepository.UpdateAsync(planilla);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _planillaRepository.ExistsAsync(planilla.Id))
                    return NotFound();
                else
                    throw;
            }

            TempData["Success"] = "Planilla actualizada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var planilla = await _planillaRepository.GetByIdAsync(id);
            if (planilla == null) return NotFound();
            return View(planilla);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _planillaRepository.DeleteAsync(id);
            TempData["Success"] = "Planilla eliminada con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}
