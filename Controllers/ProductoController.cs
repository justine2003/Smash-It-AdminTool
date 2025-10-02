using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System.Linq;

namespace SGA_Smash.Controllers;

public class ProductoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var productos = _context.Producto.Include(p => p.Categoria).ToList(); 

        return View(productos);
    }

    public IActionResult Create()
    {
        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre");
        ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Producto nuevo)
    {
        if (ModelState.IsValid)
        {
            _context.Producto.Add(nuevo);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", nuevo.CategoriaId);
        ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre", nuevo.ProveedorId);
        return View("Create", nuevo);
    }

    public IActionResult Edit(int id)
    {
        var producto = _context.Producto.Include(p => p.Categoria).FirstOrDefault(p => p.Id == id);

        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", producto.CategoriaId);
        ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre", producto.ProveedorId);
        return View(producto);
    }

    [HttpPost]
    public IActionResult Edit(int id, Producto actualizado)
    {
        var producto = _context.Producto.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();

        if (ModelState.IsValid)
        {
            producto.Nombre = actualizado.Nombre;
            producto.ProveedorId = actualizado.ProveedorId;
            producto.UnidadMedida = actualizado.UnidadMedida;
            producto.PrecioUnitario = actualizado.PrecioUnitario;
            producto.PrecioEntregaDias = actualizado.PrecioEntregaDias;
            producto.StockActual = actualizado.StockActual;
            producto.MinimoStock = actualizado.MinimoStock;
            producto.CategoriaId = actualizado.CategoriaId;
            producto.Fecha_movimiento = actualizado.Fecha_movimiento;
            producto.Estado = actualizado.Estado;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", actualizado.CategoriaId);
        return View(actualizado);
    }

    public IActionResult Detail(int id) 
    {
        var producto = _context.Producto
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefault(p => p.Id == id);

        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", producto.CategoriaId);
        ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre", producto.ProveedorId);
        return View(producto);
    }

    public IActionResult Delete(int id)
    {
        var producto = _context.Producto.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var producto = _context.Producto.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound();

        _context.Producto.Remove(producto);
        return RedirectToAction("Index");
    }
}