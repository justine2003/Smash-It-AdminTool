using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System.Threading.Tasks;

namespace SGA_Smash.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.FirstOrDefault(p => p.Nombre == model.Usuario);

                if (usuario != null) 
                {
                    bool contrasena = BCrypt.Net.BCrypt.Verify(model.Contrasena, usuario.Contrasena);

                    if (contrasena)
                    {
                        TempData["Usuario"] = model.Usuario;
                        TempData["rol"] = model.Rol;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Mensaje = "Usuario o contraseña no válido";
            return View(model);
        }

        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}