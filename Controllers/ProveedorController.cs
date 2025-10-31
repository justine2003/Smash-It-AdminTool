using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using System.Linq;


namespace SGA_Smash.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly ILogger<ProveedorController> _logger;

        public ProveedorController(IProveedorRepository proveedorRepository, ILogger<ProveedorController> logger)
        {
            _proveedorRepository = proveedorRepository;
            _logger = logger;
        }

        // LISTADO
        public async Task<IActionResult> Index(string? filtro, int pagina = 1)
        {
            const int registrosPorPagina = 20;
            var proveedores = await _proveedorRepository.GetAllProveedores();

            // Filtrar por nombre o contacto
            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLower();
                proveedores = proveedores
                    .Where(p => (p.Nombre != null && p.Nombre.ToLower().Contains(filtro)) ||
                                (p.Contacto != null && p.Contacto.ToLower().Contains(filtro)))
                    .ToList();
            }

            // Ordenar alfabéticamente por nombre
            proveedores = proveedores.OrderBy(p => p.Nombre).ToList();

            // Paginación
            var totalRegistros = proveedores.Count();
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);
            proveedores = proveedores
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Filtro = filtro;


            return View(proveedores);
        }

        // GET: CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _proveedorRepository.AddProveedor(proveedor);
                    TempData["Success"] = "Proveedor creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear proveedor");
                    TempData["Error"] = "Error al crear el proveedor. Intente nuevamente.";
                }
            }

            // Si falla la validación, mostrar errores
            var allErrors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            if (!string.IsNullOrWhiteSpace(allErrors))
            {
                TempData["Error"] = allErrors;
            }

            return View(proveedor);
        }

        // GET: EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var proveedor = await _proveedorRepository.GetProveedorById(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _proveedorRepository.UpdateProveedor(proveedor);
                    TempData["Success"] = "Proveedor actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar proveedor");
                    TempData["Error"] = "Error al actualizar el proveedor. Intente nuevamente.";
                }
            }

            return View(proveedor);
        }

        // GET: DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _proveedorRepository.GetProveedorById(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        // POST: DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _proveedorRepository.DeleteProveedor(id);
                TempData["Success"] = "Proveedor eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proveedor");
                TempData["Error"] = "Error al eliminar el proveedor. Puede estar siendo utilizado en otras partes del sistema.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}