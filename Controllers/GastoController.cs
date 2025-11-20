using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using SGA_Smash.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGA_Smash.Data;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SGA_Smash.Controllers;

public class GastoController : Controller
{
    private readonly ApplicationDbContext _context;

    public GastoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var Gasto = _context.Gasto.Include(g => g.RegistroEmpleado).ToList();

        return View(Gasto);
    }

    public IActionResult Create()
    {
        ViewBag.Empleados = new SelectList(_context.Empleados.ToList(), "Id", "Nombre");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Gasto nuevo)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Empleados = new SelectList(_context.Empleados);
            return View(nuevo);
        }

        nuevo.fecha = DateTime.Now;
        _context.Gasto.Add(nuevo);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var gasto = _context.Gasto.FirstOrDefault(g => g.id == id);
        if (gasto == null) return NotFound();

        ViewBag.Empleados = new SelectList(_context.Empleados.ToList(), "Id", "Nombre", gasto.registrado_por);
        return View(gasto);
    }

    [HttpPost]
    public IActionResult Edit(int id, Gasto actualizado)
    {
        var gasto = _context.Gasto.FirstOrDefault(g => g.id == id);
        if (gasto == null) return NotFound();

        gasto.tipo = actualizado.tipo;
        gasto.monto = actualizado.monto;
        gasto.descripcion = actualizado.descripcion;
        gasto.registrado_por = actualizado.registrado_por;
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var Gasto = _context.Gasto.Include(g => g.RegistroEmpleado).FirstOrDefault(g => g.id == id);

        if (Gasto == null) return NotFound();
        return View(Gasto);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var gasto = _context.Gasto.FirstOrDefault(g => g.id == id);
        if (gasto == null) return NotFound();

        _context.Gasto.Remove(gasto);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ExportarExecel(DateTime? desde, DateTime? hasta, string tipo) 
    {
        var sw = Stopwatch.StartNew();

        var query = _context.Gasto.AsQueryable();

        if (desde.HasValue) query = query.Where(g => g.fecha >= desde.Value.Date);

        if (hasta.HasValue) query = query.Where(g => g.fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(tipo)) query = query.Where(g => g.tipo == tipo);

        var rows = await query
            .Include(g => g.RegistroEmpleado)
            .OrderBy(g => g.fecha)
            .Select(g => new { g.id, g.tipo, g.monto, g.fecha, g.descripcion, NombreEmpelado = g.RegistroEmpleado})
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Gasto");
        ws.Cell(1, 1).Value = "id";
        ws.Cell(1, 2).Value = "tipo";
        ws.Cell(1, 3).Value = "monto";
        ws.Cell(1, 4).Value = "fecha";
        ws.Cell(1, 5).Value = "Descripción";
        ws.Cell(1, 6).Value = "Registrado Por";

        int row = 2;
        foreach (var r in rows)
        {
            ws.Cell(row, 1).Value = r.id;
            ws.Cell(row, 2).Value = r.tipo;
            ws.Cell(row, 3).Value = r.monto;
            ws.Cell(row, 4).Value = r.fecha;
            ws.Cell(row, 5).Value = r.descripcion;
            ws.Cell(row, 6).Value = r.NombreEmpelado.Nombre;
            row++;
        }

        ws.Cell(row, 2).Value = "Total";
        ws.Cell(row, 3).FormulaA1 = $"SUM(C2:C{row-1})";
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        var bytes = ms.ToArray();

        sw.Stop();
        if (sw.ElapsedMilliseconds > 5000)Response.Headers.Add("X-Report-Warning", "Tiempo de generación superó 5s");

        var fileName = $"Gasto_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    public async Task<IActionResult> ExportPDF(DateTime? desde, DateTime? hasta, string tipo)
    {
        var sw = Stopwatch.StartNew();
        var query = _context.Gasto.AsQueryable();

        if (desde.HasValue)query = query.Where(g => g.fecha >= desde.Value.Date);

        if (hasta.HasValue)query = query.Where(g => g.fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(tipo))query = query.Where(g => g.tipo == tipo);

        var rows = await query
            .Include(g => g.RegistroEmpleado)
            .OrderBy(g => g.fecha)
            .ToListAsync();

        QuestPDF.Settings.License = LicenseType.Community;
        var culture = new CultureInfo("es-ES");

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Header().Text($"Reporte de Gasto - {desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}")
                    .SemiBold().FontSize(14).AlignCenter();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(40);
                        cols.RelativeColumn(2);  
                        cols.RelativeColumn(2);  
                        cols.RelativeColumn(2); 
                        cols.RelativeColumn(4); 
                        cols.RelativeColumn(2);  
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("id");
                        header.Cell().Element(HeaderCell).Text("tipo");
                        header.Cell().Element(HeaderCell).Text("monto");
                        header.Cell().Element(HeaderCell).Text("fecha");
                        header.Cell().Element(HeaderCell).Text("Descripción");
                        header.Cell().Element(HeaderCell).Text("Registrado Por");

                        static IContainer HeaderCell(IContainer c) => c.Background(Colors.Grey.Lighten3).Padding(4);
                    });

                    decimal total = 0;
                    foreach (var g in rows)
                    {
                        total += g.monto;
                        table.Cell().Padding(3).Text(g.id.ToString());
                        table.Cell().Padding(3).Text(g.tipo);
                        table.Cell().Padding(3).AlignRight().Text(g.monto.ToString("C2", culture));
                        table.Cell().Padding(3).Text(g.fecha.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Padding(3).Text(g.descripcion ?? "");
                        table.Cell().Padding(3).Text(g.RegistroEmpleado.Nombre);
                    }

                    table.Cell().ColumnSpan(2).Element(TotalCell).Text("Totales");
                    table.Cell().Element(TotalCell).AlignRight().Text(total.ToString("C2", culture));
                    table.Cell().Element(TotalCell).Text("");
                    table.Cell().Element(TotalCell).Text("");
                    table.Cell().Element(TotalCell).Text("");

                    static IContainer TotalCell(IContainer c) => c.BorderTop(1).PaddingTop(6).PaddingBottom(4);
                });

                page.Footer().AlignRight().Text(txt =>
                {
                    txt.Span("Generado: ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        });

        var pdfBytes = doc.GeneratePdf();

        sw.Stop();
        if (sw.ElapsedMilliseconds > 5000)
            Response.Headers.Add("X-Report-Warning", "Tiempo de generación superó 5s");

        var fileName = $"Gasto_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}