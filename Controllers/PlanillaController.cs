using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers;

public class PlanillaController : Controller
{
    private static List<Empleado> empleados = new List<Empleado>
    {
        new Empleado { Id = 1, Nombre = "Carlos Soto", Puesto = "Cocinero", SalarioBase = 450000, FechaIngreso = new DateTime(2022, 3, 15), Estado = "Activo" },
        new Empleado { Id = 2, Nombre = "Andrea Gómez", Puesto = "Mesera", SalarioBase = 350000, FechaIngreso = new DateTime(2023, 1, 10), Estado = "Activo" },
        new Empleado { Id = 3, Nombre = "Luis Mora", Puesto = "Cajero", SalarioBase = 400000, FechaIngreso = new DateTime(2021, 9, 5), Estado = "Inactivo" }
    };

    private static List<Planilla> planillas = new List<Planilla>
    {
        new Planilla { Id = 1, EmpleadoNombre = "Carlos Mora", SalarioBase = 450000, Deducciones = 25000, Bonificaciones = 10000, FechaPago = new DateTime(2024, 8, 15) },
        new Planilla { Id = 2, EmpleadoNombre = "Andrea Gómez", SalarioBase = 350000, Deducciones = 15000, Bonificaciones = 5000, FechaPago = new DateTime(2024, 8, 15) }
    };

    public IActionResult Index()
    {
        return View(planillas);
    }

    public IActionResult Create()
    {
        ViewBag.Empleados = new SelectList(empleados, "Nombre", "Nombre");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Planilla nueva)
    {
        nueva.Id = planillas.Max(p => p.Id) + 1;
        nueva.FechaPago = DateTime.Now;
        planillas.Add(nueva);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var planilla = planillas.FirstOrDefault(p => p.Id == id);
        if (planilla == null) return NotFound();

        ViewBag.Empleados = new SelectList(empleados, "Nombre", "Nombre", planilla.EmpleadoNombre);
        return View(planilla);
    }

    [HttpPost]
    public IActionResult Edit(int id, Planilla actualizada)
    {
        var planilla = planillas.FirstOrDefault(p => p.Id == id);
        if (planilla == null) return NotFound();

        planilla.EmpleadoNombre = actualizada.EmpleadoNombre;
        planilla.SalarioBase = actualizada.SalarioBase;
        planilla.Deducciones = actualizada.Deducciones;
        planilla.Bonificaciones = actualizada.Bonificaciones;
        // FechaPago no se modifica en edit

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var planilla = planillas.FirstOrDefault(p => p.Id == id);
        if (planilla == null) return NotFound();
        return View(planilla);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var planilla = planillas.FirstOrDefault(p => p.Id == id);
        if (planilla == null) return NotFound();

        planillas.Remove(planilla);
        return RedirectToAction("Index");
    }
}