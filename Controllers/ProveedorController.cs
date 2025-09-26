using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers
{
    public class ProveedorController : Controller
    {
        private static List<Proveedor> proveedores = new List<Proveedor>
        {
            new Proveedor { Id = 1, Nombre = "Distribuidora Central", Contacto = "Mario López", Telefono = "8888-1111", Correo = "mario@central.com" },
            new Proveedor { Id = 2, Nombre = "Alimentos del Valle", Contacto = "Laura Rodríguez", Telefono = "8888-2222", Correo = "laura@valle.com" },
            new Proveedor { Id = 3, Nombre = "Proveeduría La Huerta", Contacto = "Carlos Méndez", Telefono = "8888-3333", Correo = "carlos@huerta.com" }
        };

        public IActionResult Index()
        {
            return View(proveedores);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Proveedor proveedor)
        {
            proveedor.Id = proveedores.Max(p => p.Id) + 1;
            proveedores.Add(proveedor);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var proveedor = proveedores.FirstOrDefault(p => p.Id == id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost]
        public IActionResult Edit(int id, Proveedor proveedorActualizado)
        {
            var proveedor = proveedores.FirstOrDefault(p => p.Id == id);
            if (proveedor == null) return NotFound();

            proveedor.Nombre = proveedorActualizado.Nombre;
            proveedor.Contacto = proveedorActualizado.Contacto;
            proveedor.Telefono = proveedorActualizado.Telefono;
            proveedor.Correo = proveedorActualizado.Correo;

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
{
    var proveedor = proveedores.FirstOrDefault(p => p.Id == id);
    if (proveedor == null) return NotFound();
    return View(proveedor);
}

[HttpPost, ActionName("Delete")]
public IActionResult DeleteConfirmed(int id)
{
    var proveedor = proveedores.FirstOrDefault(p => p.Id == id);
    if (proveedor == null) return NotFound();

    proveedores.Remove(proveedor);
    return RedirectToAction("Index");
}
    }
}