using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;

namespace SGA_Smash.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
{
    ViewBag.EmpleadosPorRol = new Dictionary<string, int>
    {
        { "Cocineros", 3 },
        { "Meseros", 4 },
        { "Cajeros", 2 },
        { "Gerentes", 1 }
    };

    ViewBag.GastosMensuales = new Dictionary<string, decimal>
    {
        { "Enero", 120000 },
        { "Febrero", 95000 },
        { "Marzo", 110000 },
        { "Abril", 130000 }
    };

    return View();
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}