using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers;

public class InventarioController : Controller
{
    private static List<Producto> productos = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Harina", UnidadMedida = "kg", PrecioUnitario = 1200, PrecioEntregaDias = 3, StockActual = 45, MinimoStock = 10 },
        new Producto { Id = 2, Nombre = "Aceite vegetal", UnidadMedida = "L", PrecioUnitario = 2000, PrecioEntregaDias = 2, StockActual = 20, MinimoStock = 5 },
        new Producto { Id = 3, Nombre = "Salsa de tomate", UnidadMedida = "unidad", PrecioUnitario = 750, PrecioEntregaDias = 5, StockActual = 80, MinimoStock = 20 }
    };

    private static List<Proveedor> proveedores = new List<Proveedor>
    {
        new Proveedor { Id = 1, Nombre = "Distribuidora Central", Contacto = "Mario López", Telefono = "8888-1111", Correo = "mario@central.com" },
        new Proveedor { Id = 2, Nombre = "Alimentos del Valle", Contacto = "Laura Rodríguez", Telefono = "8888-2222", Correo = "laura@valle.com" },
        new Proveedor { Id = 3, Nombre = "Proveeduría La Huerta", Contacto = "Carlos Méndez", Telefono = "8888-3333", Correo = "carlos@huerta.com" }
    };

    private static List<Inventario> inventario = new List<Inventario>
    {
        new Inventario { Id = 1, ProductoNombre = "Harina", ProveedorNombre = "Distribuidora Central", Cantidad = 50, FechaIngreso = new DateTime(2024, 8, 20), Estado = "Disponible" },
        new Inventario { Id = 2, ProductoNombre = "Aceite vegetal", ProveedorNombre = "Alimentos del Valle", Cantidad = 30, FechaIngreso = new DateTime(2024, 8, 18), Estado = "En uso" },
        new Inventario { Id = 3, ProductoNombre = "Salsa de tomate", ProveedorNombre = "Proveeduría La Huerta", Cantidad = 0, FechaIngreso = new DateTime(2024, 8, 10), Estado = "Agotado" }
    };

    public IActionResult Index()
    {
        return View(inventario);
    }

    public IActionResult Create()
    {
        ViewBag.Productos = new SelectList(productos, "Nombre", "Nombre");
        ViewBag.Proveedores = new SelectList(proveedores, "Nombre", "Nombre");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Inventario nuevo)
    {
        nuevo.Id = inventario.Max(i => i.Id) + 1;
        nuevo.FechaIngreso = DateTime.Now;
        inventario.Add(nuevo);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var item = inventario.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();

        ViewBag.Productos = new SelectList(productos, "Nombre", "Nombre", item.ProductoNombre);
        ViewBag.Proveedores = new SelectList(proveedores, "Nombre", "Nombre", item.ProveedorNombre);
        return View(item);
    }

    [HttpPost]
    public IActionResult Edit(int id, Inventario actualizado)
    {
        var item = inventario.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();

        item.ProductoNombre = actualizado.ProductoNombre;
        item.ProveedorNombre = actualizado.ProveedorNombre;
        item.Cantidad = actualizado.Cantidad;
        item.Estado = actualizado.Estado;
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var item = inventario.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var item = inventario.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();

        inventario.Remove(item);
        return RedirectToAction("Index");
    }
}