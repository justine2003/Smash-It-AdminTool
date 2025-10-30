using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    public class ContratoProveedorController : Controller
    {
        private readonly IContratoProveedorRepository _contratoRepository;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContratoProveedorController(
            IContratoProveedorRepository contratoRepository,
            IProveedorRepository proveedorRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _contratoRepository = contratoRepository;
            _proveedorRepository = proveedorRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: ContratoProveedor
        public async Task<IActionResult> Index(string filtro = "", int pagina = 1, int tamanoPagina = 10, int? proveedorId = null)
        {
            var contratos = await _contratoRepository.GetAllContratoProveedoresAsync();
            
            // Filtrar por proveedor específico si se proporciona
            if (proveedorId.HasValue)
            {
                contratos = contratos.Where(c => c.ProveedorId == proveedorId.Value).ToList();
                ViewBag.ProveedorId = proveedorId.Value;
                ViewBag.ProveedorNombre = contratos.FirstOrDefault()?.Proveedor?.Nombre ?? "Proveedor";
            }
            
            // Aplicar filtro si se proporciona
            if (!string.IsNullOrEmpty(filtro))
            {
                contratos = contratos.Where(c => 
                    c.Proveedor?.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) == true ||
                    c.Estado?.Contains(filtro, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
            }

            // Paginación
            var totalRegistros = contratos.Count();
            var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
            var contratosPaginados = contratos.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina);

            ViewBag.Filtro = filtro;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(contratosPaginados);
        }

        // GET: ContratoProveedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contratoRepository.GetContratoProveedorWithProveedorAsync(id.Value);
            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        // GET: ContratoProveedor/Create
        public async Task<IActionResult> Create()
        {
            await PopulateProveedoresDropDownList();
            return View(new ContratoProveedor { Estado = "Vigente" });
        }

        // POST: ContratoProveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContratoProveedor contrato, IFormFile? archivoContrato)
        {
            try
            {
                // Establecer estado por defecto si no se proporciona
                if (string.IsNullOrEmpty(contrato.Estado))
                {
                    contrato.Estado = "Vigente";
                }

                // Validar archivo PDF si se proporciona
                if (archivoContrato != null && archivoContrato.Length > 0)
                {
                    if (!EsArchivoPDFValido(archivoContrato))
                    {
                        ModelState.AddModelError("archivoContrato", "Solo se permiten archivos PDF válidos con un tamaño máximo de 10MB.");
                        await PopulateProveedoresDropDownList(contrato.ProveedorId);
                        return View(contrato);
                    }
                    
                    contrato.RutaArchivo = await GuardarArchivo(archivoContrato);
                }

                // Validar modelo después de procesar el archivo
                if (ModelState.IsValid)
                {
                    await _contratoRepository.AddContratoProveedorAsync(contrato);
                    TempData["Success"] = "Contrato creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Mostrar errores de validación
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = $"Errores de validación: {string.Join(", ", errors)}";
                }
            }
            catch (Exception ex)
            {
                var detalle = ex.GetBaseException()?.Message ?? ex.Message;
                TempData["Error"] = $"Error al crear el contrato: {detalle}";
            }

            await PopulateProveedoresDropDownList(contrato.ProveedorId);
            return View(contrato);
        }

        // GET: ContratoProveedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contratoRepository.GetContratoProveedorByIdAsync(id.Value);
            if (contrato == null)
            {
                return NotFound();
            }

            await PopulateProveedoresDropDownList(contrato.ProveedorId);
            return View(contrato);
        }

        // POST: ContratoProveedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContratoProveedor contrato, IFormFile? archivoContrato)
        {
            if (id != contrato.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cargar el contrato existente para preservar RutaArchivo cuando no se sube uno nuevo
                    var existente = await _contratoRepository.GetContratoProveedorByIdAsync(id);
                    if (existente == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos editables
                    existente.ProveedorId = contrato.ProveedorId;
                    existente.FechaInicio = contrato.FechaInicio;
                    existente.FechaFin = contrato.FechaFin;
                    existente.MontoTotal = contrato.MontoTotal;
                    existente.Estado = string.IsNullOrEmpty(contrato.Estado) ? existente.Estado : contrato.Estado;

                    // Manejar archivo si se sube uno nuevo
                    if (archivoContrato != null && archivoContrato.Length > 0)
                    {
                        if (!EsArchivoPDFValido(archivoContrato))
                        {
                            ModelState.AddModelError("archivoContrato", "Solo se permiten archivos PDF válidos con un tamaño máximo de 10MB.");
                            await PopulateProveedoresDropDownList(existente.ProveedorId);
                            return View(existente);
                        }

                        // Eliminar archivo anterior si existe
                        if (!string.IsNullOrEmpty(existente.RutaArchivo))
                        {
                            EliminarArchivo(existente.RutaArchivo);
                        }
                        existente.RutaArchivo = await GuardarArchivo(archivoContrato);
                    }
                    // Si no se sube archivo, conservar existente.RutaArchivo tal cual

                    await _contratoRepository.UpdateContratoProveedorAsync(existente);
                    TempData["Success"] = "Contrato actualizado exitosamente.";
                }
                catch (Exception ex)
                {
                    if (!await _contratoRepository.ContratoProveedorExistsAsync(contrato.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        var detalle = ex.GetBaseException()?.Message ?? ex.Message;
                        TempData["Error"] = $"Error al actualizar el contrato: {detalle}";
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateProveedoresDropDownList(contrato.ProveedorId);
            return View(contrato);
        }

        // GET: ContratoProveedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contrato = await _contratoRepository.GetContratoProveedorWithProveedorAsync(id.Value);
            if (contrato == null)
            {
                return NotFound();
            }

            return View(contrato);
        }

        // POST: ContratoProveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrato = await _contratoRepository.GetContratoProveedorByIdAsync(id);
            if (contrato != null)
            {
                // Eliminar archivo si existe
                if (!string.IsNullOrEmpty(contrato.RutaArchivo))
                {
                    EliminarArchivo(contrato.RutaArchivo);
                }

                await _contratoRepository.DeleteContratoProveedorAsync(id);
                TempData["Success"] = "Contrato eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ContratoProveedor/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var contrato = await _contratoRepository.GetContratoProveedorWithProveedorAsync(id);
            if (contrato == null || string.IsNullOrEmpty(contrato.RutaArchivo))
            {
                TempData["Error"] = "Archivo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "contratos", contrato.RutaArchivo);
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "Archivo no encontrado en el servidor.";
                return RedirectToAction(nameof(Index));
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var proveedorNombre = contrato.Proveedor?.Nombre?.Replace(" ", "_") ?? "Proveedor";
            var fileName = $"Contrato_{proveedorNombre}_{contrato.FechaInicio:yyyyMMdd}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }

        // Métodos auxiliares
        private async Task PopulateProveedoresDropDownList(object? selectedProveedor = null)
        {
            var proveedores = await _proveedorRepository.GetAllProveedores();
            // No filtrar por Estado para evitar listas vacías
            ViewBag.Proveedores = new SelectList(proveedores.OrderBy(p => p.Nombre), "Id", "Nombre", selectedProveedor);
        }

        private async Task<string> GuardarArchivo(IFormFile archivo)
        {
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "contratos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Crear nombre de archivo más seguro
                var fileName = $"{Guid.NewGuid()}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el archivo: {ex.Message}", ex);
            }
        }

        private bool EsArchivoPDFValido(IFormFile archivo)
        {
            // Verificar extensión
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (extension != ".pdf")
                return false;

            // Verificar tamaño (10MB máximo)
            const long maxSize = 10 * 1024 * 1024; // 10MB
            if (archivo.Length > maxSize)
                return false;

            // Verificar tipo MIME (más permisivo)
            var allowedMimeTypes = new[] { "application/pdf", "application/x-pdf", "application/octet-stream" };
            if (!allowedMimeTypes.Contains(archivo.ContentType))
                return false;

            return true;
        }

        private void EliminarArchivo(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "contratos", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
