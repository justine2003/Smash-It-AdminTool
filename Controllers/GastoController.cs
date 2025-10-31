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
        var gastos = _context.Gastos.ToList();

        return View(gastos);
    }

    public IActionResult Create()
    {
        ViewBag.Empleados = new SelectList(_context.Empleados);
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

        nuevo.Fecha = DateTime.Now;
        _context.Gastos.Add(nuevo);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        ViewBag.Empleados = new SelectList(_context.Empleados.ToList(), "Id", "Nombre", gasto.RegistradoPor);
        return View(gasto);
    }

    [HttpPost]
    public IActionResult Edit(int id, Gasto actualizado)
    {
        var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        gasto.Tipo = actualizado.Tipo;
        gasto.Monto = actualizado.Monto;
        gasto.Descripcion = actualizado.Descripcion;
        gasto.RegistradoPor = actualizado.RegistradoPor;
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var gastos = _context.Gastos.FirstOrDefault(g => g.Id == id);
        if (gastos == null) return NotFound();
        return View(gastos);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
        if (gasto == null) return NotFound();

        _context.Gastos.Remove(gasto);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta, string tipo) 
    {
        var sw = Stopwatch.StartNew();

        var query = _context.Gastos.AsQueryable();

        if (desde.HasValue) query = query.Where(g => g.Fecha >= desde.Value.Date);

        if (hasta.HasValue) query = query.Where(g => g.Fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(tipo)) query = query.Where(g => g.Tipo == tipo);

        var rows = await query
            .OrderBy(g => g.Fecha)
            .Select(g => new { g.Id, g.Tipo, g.Monto, g.Fecha, g.Descripcion, g.RegistradoPor })
            .ToListAsync();

        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Gastos");
        ws.Cell(1, 1).Value = "Id";
        ws.Cell(1, 2).Value = "Tipo";
        ws.Cell(1, 3).Value = "Monto";
        ws.Cell(1, 4).Value = "Fecha";
        ws.Cell(1, 5).Value = "Descripción";
        ws.Cell(1, 6).Value = "Registrado Por";

        int row = 2;
        foreach (var r in rows)
        {
            ws.Cell(row, 1).Value = r.Id;
            ws.Cell(row, 2).Value = r.Tipo;
            ws.Cell(row, 3).Value = r.Monto;
            ws.Cell(row, 4).Value = r.Fecha;
            ws.Cell(row, 5).Value = r.Descripcion;
            ws.Cell(row, 6).Value = r.RegistradoPor;
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

        var fileName = $"Gastos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    public async Task<IActionResult> ExportPDF(DateTime? desde, DateTime? hasta, string tipo)
    {
        var sw = Stopwatch.StartNew();
        var query = _context.Gastos.AsQueryable();

        if (desde.HasValue)query = query.Where(g => g.Fecha >= desde.Value.Date);

        if (hasta.HasValue)query = query.Where(g => g.Fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(tipo))query = query.Where(g => g.Tipo == tipo);

        var rows = await query
            .OrderBy(g => g.Fecha)
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
                page.Header().Text($"Reporte de Gastos - {desde:yyyy-MM-dd} a {hasta:yyyy-MM-dd}")
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
                        header.Cell().Element(HeaderCell).Text("Id");
                        header.Cell().Element(HeaderCell).Text("Tipo");
                        header.Cell().Element(HeaderCell).Text("Monto");
                        header.Cell().Element(HeaderCell).Text("Fecha");
                        header.Cell().Element(HeaderCell).Text("Descripción");
                        header.Cell().Element(HeaderCell).Text("Registrado Por");

                        static IContainer HeaderCell(IContainer c) => c.Background(Colors.Grey.Lighten3).Padding(4);
                    });

                    decimal total = 0;
                    foreach (var g in rows)
                    {
                        total += g.Monto;
                        table.Cell().Padding(3).Text(g.Id.ToString());
                        table.Cell().Padding(3).Text(g.Tipo);
                        table.Cell().Padding(3).AlignRight().Text(g.Monto.ToString("C2", culture));
                        table.Cell().Padding(3).Text(g.Fecha.ToString("dd/MM/yyyy HH:mm"));
                        table.Cell().Padding(3).Text(g.Descripcion ?? "");
                        table.Cell().Padding(3).Text(g.RegistradoPor);
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

        var fileName = $"Gastos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}