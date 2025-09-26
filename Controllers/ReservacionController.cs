using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers;

public class ReservacionController : Controller
{
    private static List<string> empleados = new() { "Andrea", "Luis", "Carlos" };

    private static List<Reservacion> reservaciones = new List<Reservacion>
    {
        new Reservacion { Id = 1, ClienteId = 1, ClienteNombre = "Juan Pérez", FechaHora = new DateTime(2024, 8, 25, 19, 0, 0), Mesa = "M1", Estado = "Confirmada", RegistradoPor = "Andrea" },
        new Reservacion { Id = 2, ClienteId = 2, ClienteNombre = "Ana María", FechaHora = new DateTime(2024, 8, 26, 20, 30, 0), Mesa = "M3", Estado = "Pendiente", RegistradoPor = "Luis" },
        new Reservacion { Id = 3, ClienteId = 3, ClienteNombre = "Luis Fernando", FechaHora = new DateTime(2024, 8, 27, 18, 45, 0), Mesa = "M2", Estado = "Cancelada", RegistradoPor = "Andrea" }
    };

    public IActionResult Index()
    {
        return View(reservaciones);
    }

    public IActionResult Create()
{
    ViewBag.Empleados = empleados;
    return View();
}

[HttpPost]
public IActionResult Create(Reservacion r)
{
    r.Id = reservaciones.Max(x => x.Id) + 1;
    reservaciones.Add(r);
    return RedirectToAction("Index");
}

    public IActionResult Edit(int id)
{
    var r = reservaciones.FirstOrDefault(x => x.Id == id);
    if (r == null) return NotFound();
    ViewBag.Empleados = empleados;
    return View(r);
}

[HttpPost]
public IActionResult Edit(int id, Reservacion rEdit)
{
    var r = reservaciones.FirstOrDefault(x => x.Id == id);
    if (r == null) return NotFound();

    r.ClienteNombre = rEdit.ClienteNombre;
    r.FechaHora = rEdit.FechaHora;
    r.Mesa = rEdit.Mesa;
    r.Estado = rEdit.Estado;
    r.RegistradoPor = rEdit.RegistradoPor;

    return RedirectToAction("Index");
}

    public IActionResult Delete(int id)
    {
        var reservacion = reservaciones.FirstOrDefault(r => r.Id == id);
        if (reservacion == null) return NotFound();
        return View(reservacion);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var reservacion = reservaciones.FirstOrDefault(r => r.Id == id);
        if (reservacion == null) return NotFound();

        reservaciones.Remove(reservacion);
        return RedirectToAction("Index");
    }
}