using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using SGA_Smash.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            var rol = HttpContext.Session.GetInt32("Rol") ?? 0;
            return rol == 1; // asumimos 1 = Admin
        }

        private int? UsuarioActualId()
        {
            return HttpContext.Session.GetInt32("UsuarioId");
        }

        // GET: Empleado
        public async Task<IActionResult> Index()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleados = await _context.Empleados.ToListAsync();
            return View(empleados);
        }

        // GET: Empleado/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = new Empleado
            {
                Estado = "Activo",
                FechaIngreso = DateTime.Today,
                DeduccionesFijas = 0,
                BonificacionesFijas = 0
            };

            return View(empleado);
        }

        // POST: Empleado/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Empleado empleado)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(empleado);
            }

            // por si acaso queremos impedir valores negativos:
            if (empleado.DeduccionesFijas < 0)
            {
                ModelState.AddModelError("DeduccionesFijas", "Las deducciones no pueden ser negativas.");
                return View(empleado);
            }
            if (empleado.BonificacionesFijas < 0)
            {
                ModelState.AddModelError("BonificacionesFijas", "Las bonificaciones no pueden ser negativas.");
                return View(empleado);
            }

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Empleado creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

       // GET: Empleado/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        // POST: Empleado/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Empleado model)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            // SOLO datos generales
            empleado.Nombre = model.Nombre;
            empleado.Puesto = model.Puesto;
            empleado.SalarioBase = model.SalarioBase;
            empleado.FechaIngreso = model.FechaIngreso;
            empleado.Estado = model.Estado;

            // NO tocar DeduccionesFijas ni BonificacionesFijas aquí

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Empleado actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Empleado/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            return View(empleado);
        }

        // POST: Empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Empleado eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ======================
        // EDITAR DEDUCCIONES / BONOS FIJOS (pantalla específica)
        // ======================

        // GET: Empleado/EditarPlanilla/5
        [HttpGet]
        public async Task<IActionResult> EditarPlanilla(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            if (!string.Equals(empleado.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Mensaje"] = "Solo se pueden modificar deducciones y bonificaciones de empleados activos.";
                return RedirectToAction("Index");
            }

            var vm = new EditarPlanillaEmpleadoViewModel
            {
                EmpleadoId = empleado.Id,
                Nombre = empleado.Nombre,
                SalarioBase = empleado.SalarioBase,
                DeduccionesFijas = empleado.DeduccionesFijas,
                BonificacionesFijas = empleado.BonificacionesFijas
            };

            return View(vm);
        }

        // POST: Empleado/EditarPlanilla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPlanilla(EditarPlanillaEmpleadoViewModel model)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var empleado = await _context.Empleados.FindAsync(model.EmpleadoId);
            if (empleado == null)
                return NotFound();

            if (!string.Equals(empleado.Estado, "Activo", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Mensaje"] = "Solo se pueden modificar deducciones y bonificaciones de empleados activos.";
                return RedirectToAction("Index");
            }

            var cambios = false;

            if (empleado.DeduccionesFijas != model.DeduccionesFijas)
            {
                _context.HistorialCambiosPlanillas.Add(new HistorialCambiosPlanilla
                {
                    EmpleadoId = empleado.Id,
                    UsuarioId = usuarioId.Value,
                    CampoModificado = "deducciones_fijas",
                    ValorAnterior = empleado.DeduccionesFijas,
                    ValorNuevo = model.DeduccionesFijas,
                    FechaCambio = DateTime.Now
                });

                empleado.DeduccionesFijas = model.DeduccionesFijas;
                cambios = true;
            }

            if (empleado.BonificacionesFijas != model.BonificacionesFijas)
            {
                _context.HistorialCambiosPlanillas.Add(new HistorialCambiosPlanilla
                {
                    EmpleadoId = empleado.Id,
                    UsuarioId = usuarioId.Value,
                    CampoModificado = "bonificaciones_fijas",
                    ValorAnterior = empleado.BonificacionesFijas,
                    ValorNuevo = model.BonificacionesFijas,
                    FechaCambio = DateTime.Now
                });

                empleado.BonificacionesFijas = model.BonificacionesFijas;
                cambios = true;
            }

            if (!cambios)
            {
                TempData["Mensaje"] = "No se detectaron cambios.";
                return RedirectToAction("Index");
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Deducciones y bonificaciones actualizadas. Se aplican en la siguiente planilla.";
            return RedirectToAction("Index");
        }

       
        [HttpGet]
        public async Task<IActionResult> HistorialPlanilla(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            var historial = await _context.HistorialCambiosPlanillas
                .Where(h => h.EmpleadoId == id)
                .OrderByDescending(h => h.FechaCambio)
                .ToListAsync();

            // Como UsuarioId es int (no nullable), no usamos .Value ni chequeo de null
            var usuarioIds = historial
                .Select(h => h.UsuarioId)
                .Distinct()
                .ToList();

            var usuarios = await _context.Usuarios
                .Where(u => usuarioIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.nombre);

            ViewBag.EmpleadoNombre = empleado.Nombre;
            ViewBag.Usuarios = usuarios;

            return View(historial);
        }
    }

}
