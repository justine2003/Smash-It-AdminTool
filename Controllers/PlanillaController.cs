using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using SGA_Smash.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Rotativa.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SGA_Smash.Controllers
{
    public class PlanillaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanillaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            var rol = HttpContext.Session.GetInt32("Rol") ?? 0;
            return rol == 1;
        }

        // LISTA / REPORTE / HISTORIAL
        public async Task<IActionResult> Index(int? mes, int? anio)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var query = _context.Planillas
                .Include(p => p.Empleado)
                .AsQueryable();

            if (mes.HasValue && mes.Value > 0)
                query = query.Where(p => p.Mes == mes.Value);

            if (anio.HasValue && anio.Value > 0)
                query = query.Where(p => p.Anio == anio.Value);

            var planillas = await query
                .OrderByDescending(p => p.Anio)
                .ThenByDescending(p => p.Mes)
                .ThenBy(p => p.Empleado.Nombre)
                .ToListAsync();

            var vm = new PlanillaReporteViewModel
            {
                Mes = mes,
                Anio = anio,
                Planillas = planillas
            };

            return View(vm);
        }

        // GET: Planilla/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            ViewBag.Empleados = await _context.Empleados
                .Where(e => e.Estado == "Activo")
                .ToListAsync();

            return View();
        }

        // POST: Planilla/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int empleadoId, int mes, int anio)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var empleado = await _context.Empleados.FindAsync(empleadoId);
            if (empleado == null)
            {
                ModelState.AddModelError("", "Empleado no encontrado.");
            }

            if (!ModelState.IsValid || empleado == null)
            {
                ViewBag.Empleados = await _context.Empleados
                    .Where(e => e.Estado == "Activo")
                    .ToListAsync();
                return View();
            }

            var existe = await _context.Planillas
                .AnyAsync(p => p.EmpleadoId == empleadoId && p.Mes == mes && p.Anio == anio);
            if (existe)
            {
                TempData["Success"] = "Ya existe una planilla para este empleado en ese período.";
                return RedirectToAction("Index", new { mes, anio });
            }

            var salarioBase = empleado.SalarioBase ?? 0;
            var bonificaciones = empleado.BonificacionesFijas;
            var deducciones = empleado.DeduccionesFijas;
            var salarioNeto = salarioBase + bonificaciones - deducciones;

            var planilla = new Planilla
            {
                EmpleadoId = empleadoId,
                Mes = mes,
                Anio = anio,
                SalarioBase = salarioBase,
                Bonificaciones = bonificaciones,
                Deducciones = deducciones,
                SalarioNeto = salarioNeto,
                FechaRegistro = DateTime.Now
            };

            _context.Planillas.Add(planilla);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Planilla registrada correctamente.";
            return RedirectToAction("Index", new { mes, anio });
        }

        // GET: Planilla/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var planilla = await _context.Planillas
                .Include(p => p.Empleado)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planilla == null)
                return NotFound();

            return View(planilla);
        }

        // GET: Planilla/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var planilla = await _context.Planillas
                .Include(p => p.Empleado)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planilla == null)
                return NotFound();

            return View(planilla);
        }

        // POST: Planilla/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Planilla model)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            if (id != model.Id)
                return BadRequest();

            var planilla = await _context.Planillas.FindAsync(id);
            if (planilla == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // volvemos a cargar Empleado si la vista lo usa
                planilla.Empleado = await _context.Empleados.FindAsync(planilla.EmpleadoId);
                return View(planilla);
            }

            // Aquí permitimos corregir datos de la planilla específica
            planilla.Mes = model.Mes;
            planilla.Anio = model.Anio;
            planilla.SalarioBase = model.SalarioBase;
            planilla.Bonificaciones = model.Bonificaciones;
            planilla.Deducciones = model.Deducciones;
            planilla.SalarioNeto = planilla.SalarioBase + planilla.Bonificaciones - planilla.Deducciones;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Planilla actualizada correctamente.";
            return RedirectToAction("Index", new { mes = planilla.Mes, anio = planilla.Anio });
        }

        // GET: Planilla/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var planilla = await _context.Planillas
                .Include(p => p.Empleado)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (planilla == null)
                return NotFound();

            return View(planilla);
        }

        // POST: Planilla/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var planilla = await _context.Planillas.FindAsync(id);
            if (planilla == null)
                return NotFound();

            var mes = planilla.Mes;
            var anio = planilla.Anio;

            _context.Planillas.Remove(planilla);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Planilla eliminada correctamente.";
            return RedirectToAction("Index", new { mes, anio });
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPdf(int? mes, int? anio)
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var query = _context.Planillas
                .Include(p => p.Empleado)
                .AsQueryable();

            if (mes.HasValue)
                query = query.Where(p => p.Mes == mes.Value);

            if (anio.HasValue)
                query = query.Where(p => p.Anio == anio.Value);

            var planillas = await query
                .OrderBy(p => p.Anio)
                .ThenBy(p => p.Mes)
                .ThenBy(p => p.Empleado.Nombre)
                .ToListAsync();

            var vm = new PlanillaReporteViewModel
            {
                Planillas = planillas,
                Mes = mes,
                Anio = anio
            };

            var fileName = $"Planilla_{(mes ?? 0):D2}_{(anio ?? DateTime.Now.Year)}.pdf";

            return new ViewAsPdf("ReportePlanillaPdf", vm)
            {
                FileName = fileName,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
    }
}
