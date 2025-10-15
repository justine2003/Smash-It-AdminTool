using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGA_Smash.Repositories;

namespace SGA_Smash.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class PlanillaReporteController : Controller
    {
        private readonly IPlanillaReportService _service;

        public PlanillaReporteController(IPlanillaReportService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int? mes, int? anio)
        {
            var now = DateTime.Now;
            int m = mes ?? now.Month;
            int y = anio ?? now.Year;

            var sw = Stopwatch.StartNew();
            var result = await _service.GetMonthlyAsync(m, y);
            sw.Stop();
            ViewBag.ElapsedMs = sw.ElapsedMilliseconds;

            return View(result);
        }

        public async Task<IActionResult> ExportExcel(int mes, int anio)
        {
            var sw = Stopwatch.StartNew();
            var result = await _service.GetMonthlyAsync(mes, anio);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet($"Planilla {mes:00}-{anio}");
            ws.Cell(1, 1).Value = "Empleado";
            ws.Cell(1, 2).Value = "Salario Bruto";
            ws.Cell(1, 3).Value = "Deducciones";
            ws.Cell(1, 4).Value = "Bonificaciones";
            ws.Cell(1, 5).Value = "Salario Neto";

            int row = 2;
            foreach (var r in result.Rows)
            {
                ws.Cell(row, 1).Value = r.EmpleadoNombre;
                ws.Cell(row, 2).Value = r.SalarioBruto;
                ws.Cell(row, 3).Value = r.Deducciones;
                ws.Cell(row, 4).Value = r.Bonificaciones;
                ws.Cell(row, 5).Value = r.SalarioNeto;
                row++;
            }

            ws.Cell(row, 1).Value = "Totales";
            ws.Cell(row, 2).FormulaA1 = $"SUM(B2:B{row-1})";
            ws.Cell(row, 3).FormulaA1 = $"SUM(C2:C{row-1})";
            ws.Cell(row, 4).FormulaA1 = $"SUM(D2:D{row-1})";
            ws.Cell(row, 5).FormulaA1 = $"SUM(E2:E{row-1})";
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            sw.Stop();
            if (sw.ElapsedMilliseconds > 5000)
                Response.Headers.Add("X-Report-Warning", "Tiempo de generación superó 5s");

            var fileName = $"Planilla_{anio}_{mes:00}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> ExportPdf(int mes, int anio)
        {
            var sw = Stopwatch.StartNew();
            var result = await _service.GetMonthlyAsync(mes, anio);

            QuestPDF.Settings.License = LicenseType.Community;
            var culture = new CultureInfo("es-CR");

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.Header().Text($"Reporte de Planilla - {mes:00}/{anio}")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(4); // Empleado
                            c.RelativeColumn(2); // Bruto
                            c.RelativeColumn(2); // Deducciones
                            c.RelativeColumn(2); // Bonif
                            c.RelativeColumn(2); // Neto
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(CellHeader).Text("Empleado");
                            h.Cell().Element(CellHeader).Text("Salario Bruto");
                            h.Cell().Element(CellHeader).Text("Deducciones");
                            h.Cell().Element(CellHeader).Text("Bonificaciones");
                            h.Cell().Element(CellHeader).Text("Salario Neto");

                            static IContainer CellHeader(IContainer container) =>
                                container.Background(Colors.Grey.Lighten3).Padding(4).DefaultTextStyle(t => t.SemiBold());
                        });

                        foreach (var r in result.Rows)
                        {
                            table.Cell().PaddingVertical(2).Text(r.EmpleadoNombre);
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.SalarioBruto.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.Deducciones.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.Bonificaciones.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.SalarioNeto.ToString("C0", culture));
                        }

                        table.Cell().Element(CellTotal).Text("Totales");
                        table.Cell().Element(CellTotal).AlignRight().Text(result.TotalBruto.ToString("C0", culture));
                        table.Cell().Element(CellTotal).AlignRight().Text(result.TotalDeducciones.ToString("C0", culture));
                        table.Cell().Element(CellTotal).AlignRight().Text(result.TotalBonificaciones.ToString("C0", culture));
                        table.Cell().Element(CellTotal).AlignRight().Text(result.TotalNeto.ToString("C0", culture));

                        static IContainer CellTotal(IContainer c) => c.BorderTop(1).PaddingTop(4).PaddingBottom(2);
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

            var fileName = $"Planilla_{anio}_{mes:00}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
