using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using System.Linq;

namespace SGA_Smash.Controllers;

public class ProductoController : Controller
{
    private static List<Producto> productos = new List<Producto>
    {
        new Producto { Id = 1, Nombre = "Harina", UnidadMedida = "kg", PrecioUnitario = 1200, PrecioEntregaDias = 3, StockActual = 45, MinimoStock = 10 },
        new Producto { Id = 2, Nombre = "Aceite vegetal", UnidadMedida = "L", PrecioUnitario = 2000, PrecioEntregaDias = 2, StockActual = 20, MinimoStock = 5 },
        new Producto { Id = 3, Nombre = "Salsa de tomate", UnidadMedida = "unidad", PrecioUnitario = 750, PrecioEntregaDias = 5, StockActual = 80, MinimoStock = 20 }
    };

    public IActionResult Index()
    {
        return View(productos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Producto nuevo)
    {
        nuevo.Id = productos.Max(p => p.Id) + 1;
        productos.Add(nuevo);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost]
    public IActionResult Edit(int id, Producto actualizado)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();

        producto.Nombre = actualizado.Nombre;
        producto.UnidadMedida = actualizado.UnidadMedida;
        producto.PrecioUnitario = actualizado.PrecioUnitario;
        producto.PrecioEntregaDias = actualizado.PrecioEntregaDias;
        producto.StockActual = actualizado.StockActual;
        producto.MinimoStock = actualizado.MinimoStock;

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();

        productos.Remove(producto);
        return RedirectToAction("Index");
    }
}