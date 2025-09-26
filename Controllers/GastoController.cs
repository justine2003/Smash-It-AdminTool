using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SGA_Smash.Controllers;

public class GastoController : Controller
{
    private static List<Gasto> gastos = new List<Gasto>
    {
        new Gasto { Id = 1, Tipo = "Servicios", Monto = 25000, Descripcion = "Pago de electricidad", Fecha = new DateTime(2024, 8, 1), RegistradoPor = "Carlos" },
        new Gasto { Id = 2, Tipo = "Insumos", Monto = 15000, Descripcion = "Compra de servilletas", Fecha = new DateTime(2024, 8, 2), RegistradoPor = "Andrea" },
        new Gasto { Id = 3, Tipo = "Mantenimiento", Monto = 50000, Descripcion = "Reparación de horno", Fecha = new DateTime(2024, 8, 3), RegistradoPor = "Mario" }
    };

    private static List<string> empleados = new List<string> { "Carlos", "Andrea", "Mario" };

    public IActionResult Index()
    {
        return View(gastos);
    }

    public IActionResult Create()
    {
        ViewBag.Empleados = new SelectList(empleados);
        return View();
    }

    [HttpPost]
    public IActionResult Create(Gasto nuevo)
    {
        nuevo.Id = gastos.Max(g => g.Id) + 1;
        nuevo.Fecha = DateTime.Now;
        gastos.Add(nuevo);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var gasto = gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        ViewBag.Empleados = new SelectList(empleados, gasto.RegistradoPor);
        return View(gasto);
    }

    [HttpPost]
    public IActionResult Edit(int id, Gasto actualizado)
    {
        var gasto = gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        gasto.Tipo = actualizado.Tipo;
        gasto.Monto = actualizado.Monto;
        gasto.Descripcion = actualizado.Descripcion;
        gasto.RegistradoPor = actualizado.RegistradoPor;

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var gasto = gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();
        return View(gasto);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var gasto = gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        gastos.Remove(gasto);
        return RedirectToAction("Index");
    }
}