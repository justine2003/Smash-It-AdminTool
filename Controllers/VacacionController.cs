using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    public class VacacionController : Controller
    {
        private readonly IVacacionRepository _vacacionRepository;
        private readonly IEmpleadoRepository _empleadoRepository;

        public VacacionController(IVacacionRepository vacacionRepository, IEmpleadoRepository empleadoRepository)
        {
            _vacacionRepository = vacacionRepository;
            _empleadoRepository = empleadoRepository;
        }

        private async Task LoadCombosAsync(int? empleadoId = null, int? aprobadorId = null, string estado = null)
        {
            var empleados = await _empleadoRepository.GetAllEmpleadosAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", empleadoId);
            ViewBag.Aprobadores = new SelectList(empleados, "Id", "Nombre", aprobadorId);

            var estados = new[] { "Pendiente", "Aprobada", "Rechazada" };
            ViewBag.Estados = new SelectList(estados, estado);
        }

        public async Task<IActionResult> Index()
        {
            var items = await _vacacionRepository.GetAllAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _vacacionRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCombosAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vacacion vacacion)
        {
            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(vacacion.EmpleadoId, vacacion.AprobadoPor, vacacion.Estado);
                return View(vacacion);
            }

            await _vacacionRepository.AddAsync(vacacion);
            TempData["Success"] = "Solicitud de vacaciones registrada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _vacacionRepository.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadCombosAsync(item.EmpleadoId, item.AprobadoPor, item.Estado);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vacacion vacacion)
        {
            if (id != vacacion.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCombosAsync(vacacion.EmpleadoId, vacacion.AprobadoPor, vacacion.Estado);
                return View(vacacion);
            }

            try
            {
                await _vacacionRepository.UpdateAsync(vacacion);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _vacacionRepository.ExistsAsync(vacacion.Id)) return NotFound();
                else throw;
            }

            TempData["Success"] = "Solicitud de vacaciones actualizada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _vacacionRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vacacionRepository.DeleteAsync(id);
            TempData["Success"] = "Solicitud de vacaciones eliminada con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}
