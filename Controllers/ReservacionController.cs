
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservacionController> _logger;

        public ReservacionController(ApplicationDbContext context, ILogger<ReservacionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var reservaciones = await _context.Reservaciones
                .Include(r => r.Cliente)
                .ToListAsync();

            return View(reservaciones);
        }

        // GET: CREATE
        public async Task<IActionResult> Create()
        {
            var clientes = await _context.Clientes.ToListAsync();
            ViewBag.Clientes = clientes;
            _logger.LogInformation("Create GET - Clientes disponibles: {Count}", clientes.Count);
            if (clientes.Count == 0)
            {
                TempData["Error"] = "No hay clientes registrados. Registra al menos uno antes de crear una reservación.";
            }
            return View(new Reservacion());
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservacion reservacion)
        {
            _logger.LogInformation("Create POST - ClienteId recibido: {ClienteId}", reservacion.ClienteId);
            _logger.LogInformation("Create POST - FechaHora recibida: {FechaHora}", reservacion.FechaHora);
            _logger.LogInformation("Create POST - Mesa recibida: {Mesa}", reservacion.Mesa);
            _logger.LogInformation("Create POST - Estado recibido: {Estado}", reservacion.Estado);

            // Validaciones adicionales
            if (reservacion.ClienteId <= 0)
            {
                ModelState.AddModelError(nameof(reservacion.ClienteId), "Debe seleccionar un cliente");
                _logger.LogWarning("ClienteId inválido: {ClienteId}", reservacion.ClienteId);
            }

            // Si no se envía fecha, quedaría como DateTime.MinValue, inválido en SQL Server
            if (reservacion.FechaHora == default)
            {
                ModelState.AddModelError(nameof(reservacion.FechaHora), "Debe ingresar fecha y hora válidas");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manejo de FK registrado_por: si no existe Usuario 1, permitir null
                    reservacion.RegistradoPor = 1;
                    _context.Reservaciones.Add(reservacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Reservación creada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    _logger.LogWarning("Fallo al guardar con RegistradoPor=1. Reintentando con null.");
                    // Reintentar sin registrado_por si hay error de FK
                    _context.ChangeTracker.Clear();
                    reservacion.RegistradoPor = null;
                    _context.Reservaciones.Add(reservacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Reservación creada (sin usuario registrado).";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Si falla la validación, volvemos a cargar la lista de clientes
            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            var allErrors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            if (!string.IsNullOrWhiteSpace(allErrors))
            {
                TempData["Error"] = allErrors;
            }
            return View(reservacion);
        }

        // GET: EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null) return NotFound();

            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            return View(reservacion);
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservacion reservacion)
        {
            if (id != reservacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Reservación actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar reservación");
                    TempData["Error"] = "Error al actualizar la reservación. Intente nuevamente.";
                }
            }

            ViewBag.Clientes = await _context.Clientes.ToListAsync();
            return View(reservacion);
        }

        // GET: DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var reservacion = await _context.Reservaciones
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservacion == null) return NotFound();
            return View(reservacion);
        }

        // POST: DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var reservacion = await _context.Reservaciones.FindAsync(id);
                if (reservacion == null) return NotFound();

                _context.Reservaciones.Remove(reservacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reservación eliminada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reservación");
                TempData["Error"] = "Error al eliminar la reservación. Intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
