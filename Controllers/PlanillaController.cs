using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using SGA_Smash.Services;

namespace SGA_Smash.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PlanillaController : Controller
    {
        private readonly IPlanillaRepository _planillas;
        private readonly IEmpleadoRepository _empleados;
        private readonly PlanillaCalculoService _calculo;
        private readonly ReportService _reportes;

        public PlanillaController(
            IPlanillaRepository planillas,
            IEmpleadoRepository empleados,
            PlanillaCalculoService calculo,
            ReportService reportes)
        {
            _planillas = planillas;
            _empleados = empleados;
            _calculo = calculo;
            _reportes = reportes;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _planillas.GetAllAsync();
            return View(items);
        }

        public async Task<IActionResult> Create()
        {
            var empleados = await _empleados.GetAllAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre");
            var today = DateTime.Today;
            return View(new Planilla { Mes = today.Month, Anio = today.Year });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Planilla model, bool autocalcular = true)
        {
            if (autocalcular)
            {
                var (ded, bon) = await _calculo.CalcularActivosAsync(model.EmpleadoId);
                if (model.Deducciones <= 0) model.Deducciones = ded;
                if (model.Bonificaciones <= 0) model.Bonificaciones = bon;
            }

            if (await _planillas.ExistsPeriodoAsync(model.EmpleadoId, model.Mes, model.Anio))
                ModelState.AddModelError(string.Empty, "Ya existe una planilla para este empleado en el período indicado.");

            if (!ModelState.IsValid)
            {
                var empleados = await _empleados.GetAllAsync();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", model.EmpleadoId);
                return View(model);
            }

            await _planillas.AddAsync(model);
            TempData["Success"] = "Planilla registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _planillas.GetByIdAsync(id);
            if (p == null) return NotFound();
            var empleados = await _empleados.GetAllAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", p.EmpleadoId);
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Planilla model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var empleados = await _empleados.GetAllAsync();
                ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", model.EmpleadoId);
                return View(model);
            }

            await _planillas.UpdateAsync(model);
            TempData["Success"] = "Planilla actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _planillas.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _planillas.DeleteAsync(id);
            TempData["Success"] = "Planilla eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Historial (filtrable)
        public async Task<IActionResult> Historial(int? mes, int? anio)
        {
            var items = await _planillas.GetAllAsync();
            if (mes.HasValue) items = items.Where(p => p.Mes == mes.Value);
            if (anio.HasValue) items = items.Where(p => p.Anio == anio.Value);
            return View(items.OrderByDescending(p => p.Anio).ThenByDescending(p => p.Mes));
        }

        // Reporte mensual + totales
        public async Task<IActionResult> Reporte(int mes, int anio)
        {
            var (items, bruto, ded, bon, neto) = await _reportes.GetMensualAsync(mes, anio);
            ViewBag.Mes = mes;
            ViewBag.Anio = anio;
            ViewBag.TotalBruto = bruto;
            ViewBag.TotalDeducciones = ded;
            ViewBag.TotalBonificaciones = bon;
            ViewBag.TotalNeto = neto;
            return View(items);
        }

        // Export Excel (CSV sencillo compatible)
        public async Task<FileResult> ExportExcel(int mes, int anio)
        {
            var (items, _, _, _, _) = await _reportes.GetMensualAsync(mes, anio);
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Empleado,Mes,Anio,SalarioBase,Deducciones,Bonificaciones,SalarioNeto");
            foreach (var p in items)
            {
                var nombre = p.Empleado?.Nombre ?? $"Empleado #{p.EmpleadoId}";
                csv.AppendLine($"{nombre},{p.Mes},{p.Anio},{p.SalarioBase},{p.Deducciones},{p.Bonificaciones},{p.SalarioNeto}");
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fname = $"reporte_planilla_{anio}_{mes:00}.csv";
            return File(bytes, "text/csv", fname);
        }
    }
}
