using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public EmpleadoController(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var empleados = await _empleadoRepository.GetAllEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult Details(int id)
        {
            var empleado = _empleadoRepository.GetEmpleadoByIdAsync(id).Result;
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                await _empleadoRepository.AddEmpleadoAsync(empleado);
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await _empleadoRepository.GetEmpleadoByIdAsync(id);
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Empleado empleado)
        {
            if (id != empleado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _empleadoRepository.UpdateEmpleadoAsync(empleado);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    if (!await _empleadoRepository.EmpleadoExistsAsync(empleado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var empleado = await _empleadoRepository.GetEmpleadoByIdAsync(id);
            if (empleado == null) return NotFound();
            return View(empleado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _empleadoRepository.DeleteEmpleadoAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}