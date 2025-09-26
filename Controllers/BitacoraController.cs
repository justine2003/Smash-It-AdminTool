using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using System.Collections.Generic;

namespace SGA_Smash.Controllers
{
    public class BitacoraController : Controller
    {
        private static List<Bitacora> bitacoras = new List<Bitacora>
        {
            new Bitacora { Id = 1, Usuario = "admin", Modulo = "Empleados", Accion = "Crear", Fecha = new DateTime(2024, 8, 1), Detalles = "Se registró nuevo empleado: Carlos Soto" },
            new Bitacora { Id = 2, Usuario = "admin", Modulo = "Reservaciones", Accion = "Eliminar", Fecha = new DateTime(2024, 8, 2), Detalles = "Reservación eliminada para cliente Juan Pérez" },
            new Bitacora { Id = 3, Usuario = "soporte", Modulo = "Inventario", Accion = "Editar", Fecha = new DateTime(2024, 8, 3), Detalles = "Se actualizó cantidad de Harina" },
            new Bitacora { Id = 4, Usuario = "admin", Modulo = "Productos", Accion = "Crear", Fecha = new DateTime(2024, 8, 4), Detalles = "Producto nuevo agregado: Salsa BBQ" },
            new Bitacora { Id = 5, Usuario = "jose", Modulo = "Clientes", Accion = "Editar", Fecha = new DateTime(2024, 8, 5), Detalles = "Actualizado número telefónico de Ana María" },
            new Bitacora { Id = 6, Usuario = "laura", Modulo = "Gastos", Accion = "Registrar", Fecha = new DateTime(2024, 8, 6), Detalles = "Registrado gasto de ₡75,000 por compra de gas" },
            new Bitacora { Id = 7, Usuario = "soporte", Modulo = "Planilla", Accion = "Pagar", Fecha = new DateTime(2024, 8, 7), Detalles = "Pagado salario de mes julio a Andrea Gómez" },
            new Bitacora { Id = 8, Usuario = "admin", Modulo = "Vacaciones", Accion = "Aprobar", Fecha = new DateTime(2024, 8, 8), Detalles = "Vacaciones aprobadas para Luis Mora" },
            new Bitacora { Id = 9, Usuario = "soporte", Modulo = "Inventario", Accion = "Eliminar", Fecha = new DateTime(2024, 8, 9), Detalles = "Se eliminó registro duplicado de Aceite vegetal" },
            new Bitacora { Id = 10, Usuario = "jose", Modulo = "Proveedores", Accion = "Actualizar", Fecha = new DateTime(2024, 8, 10), Detalles = "Correo electrónico actualizado de Distribuidora Central" }
        };

        public IActionResult Index()
        {
            return View(bitacoras.OrderByDescending(b => b.Fecha).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Bitacora nueva)
        {
            nueva.Id = bitacoras.Max(b => b.Id) + 1;
            nueva.Fecha = DateTime.Now;
            bitacoras.Add(nueva);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var bitacora = bitacoras.FirstOrDefault(b => b.Id == id);
            if (bitacora == null) return NotFound();
            return View(bitacora);
        }

        [HttpPost]
        public IActionResult Edit(int id, Bitacora actualizada)
        {
            var bitacora = bitacoras.FirstOrDefault(b => b.Id == id);
            if (bitacora == null) return NotFound();

            bitacora.Usuario = actualizada.Usuario;
            bitacora.Modulo = actualizada.Modulo;
            bitacora.Accion = actualizada.Accion;
            bitacora.Detalles = actualizada.Detalles;
            // Fecha no se modifica

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var bitacora = bitacoras.FirstOrDefault(b => b.Id == id);
            if (bitacora == null) return NotFound();
            return View(bitacora);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var bitacora = bitacoras.FirstOrDefault(b => b.Id == id);
            if (bitacora == null) return NotFound();

            bitacoras.Remove(bitacora);
            return RedirectToAction("Index");
        }
    }
}