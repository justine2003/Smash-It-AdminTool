using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // 👈 para Session
using SGA_Smash.Data;
using SGA_Smash.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

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
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensaje = "Usuario o contraseña no válido";
                return View(model);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(p => p.nombre == model.nombre);

            if (usuario != null)
            {
                try
                {
                    bool contrasenaOk = BCrypt.Net.BCrypt.Verify(model.contrasena, usuario.contrasena);

                    if (contrasenaOk)
                    {
                        HttpContext.Session.SetString("Usuario", usuario.nombre);
                        HttpContext.Session.SetInt32("Rol", usuario.rol_id ?? 0);

                        // (TempData lo puedes seguir usando para mensajes si querés)
                        TempData["Usuario"] = usuario.nombre;

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception)
                {
                }
            }

            ViewBag.Mensaje = "Usuario o contraseña no válido";
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new Usuario());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeUsuario = await _context.Usuarios
                .AnyAsync(u => u.nombre == model.nombre);

            if (existeUsuario)
            {
                ModelState.AddModelError("nombre", "Ya existe un usuario con ese nombre.");
                return View(model);
            }

            var usuario = new Usuario
            {
                nombre = model.nombre,
                correo = model.correo,
                contrasena = BCrypt.Net.BCrypt.HashPassword(model.contrasena),
                fecha_creacion = DateTime.Now,
                ultimo_acceso = null,
                rol_id = model.rol_id ?? 2
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("Usuario", usuario.nombre);
            HttpContext.Session.SetInt32("Rol", usuario.rol_id ?? 0);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            TempData.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

