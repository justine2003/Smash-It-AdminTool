using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using System.Threading.Tasks;
using System.Linq;

namespace SGA_Smash.Controllers
{
    public class HistorialCambiosPlanillaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialCambiosPlanillaController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            var rol = HttpContext.Session.GetInt32("Rol") ?? 0;
            return rol == 1;
        }

        public async Task<IActionResult> Index()
        {
            if (!EsAdmin())
                return RedirectToAction("Index", "Home");

            var historial = await _context.HistorialCambiosPlanillas
                .Include(h => h.Empleado)
                .Include(h => h.Usuario)
                .OrderByDescending(h => h.FechaCambio)
                .ToListAsync();

            return View(historial);
        }
    }
}
