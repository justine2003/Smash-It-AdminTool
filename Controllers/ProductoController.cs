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
        var productoOriginal = _context.Producto.AsNoTracking().FirstOrDefault(p => p.Id == id);
        if (productoOriginal == null) return NotFound();

        if (ModelState.IsValid)
        {
            int stockAnterior = productoOriginal.StockActual;
            int nuevoStock = actualizado.StockActual;
            bool stockDescontadoEnMetodoStock = false;

            if (nuevoStock < stockAnterior)
            {
                string errorMessage;

                if (!Stock(id, nuevoStock, out errorMessage))
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", actualizado.CategoriaId);
                    ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre", actualizado.ProveedorId);
                    return View(actualizado);
                }
                stockDescontadoEnMetodoStock = true;
            }

            var productoAActualizar = _context.Producto.FirstOrDefault(p => p.Id == id);
            if (productoAActualizar == null) return NotFound();

            productoAActualizar.Nombre = actualizado.Nombre;
            productoAActualizar.ProveedorId = actualizado.ProveedorId;
            productoAActualizar.UnidadMedida = actualizado.UnidadMedida;
            productoAActualizar.PrecioUnitario = actualizado.PrecioUnitario;
            productoAActualizar.PrecioEntregaDias = actualizado.PrecioEntregaDias;
            productoAActualizar.MinimoStock = actualizado.MinimoStock;
            productoAActualizar.CategoriaId = actualizado.CategoriaId;
            productoAActualizar.Estado = actualizado.Estado;

            if (nuevoStock >= stockAnterior)
            {
                productoAActualizar.StockActual = nuevoStock;
                productoAActualizar.Fecha_movimiento = DateTime.Now;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        ViewBag.CategoriaID = new SelectList(_context.Categoria, "Id", "Nombre", actualizado.CategoriaId);
        ViewBag.ProveedorID = new SelectList(_context.Proveedor, "Id", "Nombre", actualizado.ProveedorId);
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

    public bool Stock(int productoId, int cantidad, out string errorMessage)
    {
        errorMessage = string.Empty;

        var producto = _context.Producto.FirstOrDefault(p => p.Id == productoId);

        if (producto == null)
        {
            errorMessage = "Producto no encontrado.";
            return false;
        }

        if (producto.StockActual < cantidad)
        {
            errorMessage = "Stock insuficiente.";
            return false;
        }

        producto.StockActual -= cantidad;
        producto.Fecha_movimiento = DateTime.Now;

        if (producto.StockActual <= producto.MinimoStock)
        {
            string alerta = $"Alerta: El stock del producto '{producto.Nombre}' ha alcanzado el nivel mínimo.";

            var Notificacion = new Notificacion
            {
                Mensaje = alerta,
                Fecha = DateTime.Now,
                Tipo = "Alerta"
            };
            _context.Add(Notificacion);
        }

        try
        {
            _context.Update(producto);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Error al actualizar el stock: {ex.Message}";
            return false;
        }
    }
}