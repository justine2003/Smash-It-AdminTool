using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers
{
    public class AccountController : Controller
    {
        // Datos quemados de usuarios
        private static readonly List<string> UsuariosPermitidos = new List<string> { "admin", "gerente", "cajero" };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid && UsuariosPermitidos.Contains(model.Usuario.ToLower()))
            {
                TempData["Usuario"] = model.Usuario;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Mensaje = "Usuario no válido";
            return View(model);
        }

        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}