using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System;
using System.Collections.Generic;

namespace SGA_Smash.Controllers
{
    public class NotificacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
           var notificaciones = _context.Notificacion.OrderByDescending(p => p.Fecha).ToList();

            return View(notificaciones);
        }
    }
}