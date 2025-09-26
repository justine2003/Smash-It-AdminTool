using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers;

public class VacacionController : Controller
{
    private static List<Empleado> empleados = new List<Empleado>
    {
        new Empleado { Id = 1, Nombre = "Carlos Soto", Puesto = "Cocinero", SalarioBase = 450000, FechaIngreso = new DateTime(2022, 3, 15), Estado = "Activo" },
        new Empleado { Id = 2, Nombre = "Andrea Gómez", Puesto = "Mesera", SalarioBase = 350000, FechaIngreso = new DateTime(2023, 1, 10), Estado = "Activo" },
        new Empleado { Id = 3, Nombre = "Luis Mora", Puesto = "Cajero", SalarioBase = 400000, FechaIngreso = new DateTime(2021, 9, 5), Estado = "Inactivo" }
    };

    private static List<Vacacion> vacaciones = new List<Vacacion>
    {
        new Vacacion { Id = 1, EmpleadoNombre = "Carlos Soto", FechaInicio = new DateTime(2024, 7, 1), FechaFin = new DateTime(2024, 7, 15), Estado = "Aprobado" },
        new Vacacion { Id = 2, EmpleadoNombre = "Andrea Gómez", FechaInicio = new DateTime(2024, 8, 5), FechaFin = new DateTime(2024, 8, 10), Estado = "Pendiente" }
    };

    public IActionResult Index()
    {
        return View(vacaciones);
    }

    public IActionResult Create()
    {
        ViewBag.Empleados = new SelectList(empleados.Select(e => e.Nombre));
        return View();
    }

    [HttpPost]
    public IActionResult Create(Vacacion vacacion)
    {
        vacacion.Id = vacaciones.Max(v => v.Id) + 1;
        vacaciones.Add(vacacion);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var vacacion = vacaciones.FirstOrDefault(v => v.Id == id);
        if (vacacion == null) return NotFound();
        ViewBag.Empleados = new SelectList(empleados.Select(e => e.Nombre));
        return View(vacacion);
    }

    [HttpPost]
    public IActionResult Edit(int id, Vacacion actualizado)
    {
        var vacacion = vacaciones.FirstOrDefault(v => v.Id == id);
        if (vacacion == null) return NotFound();

        vacacion.EmpleadoNombre = actualizado.EmpleadoNombre;
        vacacion.FechaInicio = actualizado.FechaInicio;
        vacacion.FechaFin = actualizado.FechaFin;
        vacacion.Estado = actualizado.Estado;
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var vacacion = vacaciones.FirstOrDefault(v => v.Id == id);
        if (vacacion == null) return NotFound();
        return View(vacacion);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var vacacion = vacaciones.FirstOrDefault(v => v.Id == id);
        if (vacacion == null) return NotFound();
        vacaciones.Remove(vacacion);
        return RedirectToAction("Index");
    }
}