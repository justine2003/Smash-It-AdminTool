using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using System;
using System.Collections.Generic;

namespace SGA_Smash.Controllers
{
    public class NotificacionController : Controller
    {
        public IActionResult Index()
        {
            var notificaciones = new List<Notificacion>
            {
                new Notificacion { Id = 1, Mensaje = "Producto 'Pan Brioche' agotado.", Fecha = DateTime.Now.AddHours(-1), Tipo = "Alerta" },
                new Notificacion { Id = 2, Mensaje = "Inventario de 'Carne Smash' bajo.", Fecha = DateTime.Now.AddHours(-3), Tipo = "Alerta" },
                new Notificacion { Id = 3, Mensaje = "Se actualizó el inventario correctamente.", Fecha = DateTime.Now.AddDays(-1), Tipo = "Info" }
            };

            return View(notificaciones);
        }
    }
}