using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGA_Smash.Repositories; // para IEmpleadoRepository

namespace SGA_Smash.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class HistorialPagosController : Controller
    {
        private readonly IHistorialPagosService _service;
        private readonly IEmpleadoRepository _empleadoRepository;

        public HistorialPagosController(IHistorialPagosService service, IEmpleadoRepository empleadoRepository)
        {
            _service = service;
            _empleadoRepository = empleadoRepository;
        }

        private async Task LoadEmpleadosAsync(int? selected = null)
        {
            var empleados = await _empleadoRepository.GetAllEmpleadosAsync();
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombre", selected);
        }

        // GET: /HistorialPagos
        public async Task<IActionResult> Index(int? mesDesde, int? anioDesde, int? mesHasta, int? anioHasta, int? empleadoId)
        {
            var now = DateTime.Now;
            // Por defecto: últimos 12 meses
            int md = mesDesde ?? (now.AddMonths(-11).Month);
            int yd = anioDesde ?? (now.AddMonths(-11).Year);
            int mh = mesHasta ?? now.Month;
            int yh = anioHasta ?? now.Year;

            var sw = Stopwatch.StartNew();
            var result = await _service.GetAsync(md, yd, mh, yh, empleadoId);
            sw.Stop();
            ViewBag.ElapsedMs = sw.ElapsedMilliseconds;

            await LoadEmpleadosAsync(empleadoId);
            return View(result);
        }

        public async Task<IActionResult> ExportExcel(int mesDesde, int anioDesde, int mesHasta, int anioHasta, int? empleadoId)
        {
            var result = await _service.GetAsync(mesDesde, anioDesde, mesHasta, anioHasta, empleadoId);

            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("HistorialPagos");

            ws.Cell(1, 1).Value = "Periodo";
            ws.Cell(1, 2).Value = "Empleado";
            ws.Cell(1, 3).Value = "Salario Bruto";
            ws.Cell(1, 4).Value = "Deducciones";
            ws.Cell(1, 5).Value = "Bonificaciones";
            ws.Cell(1, 6).Value = "Salario Neto";

            int row = 2;
            foreach (var r in result.Rows)
            {
                ws.Cell(row, 1).Value = $"{r.Mes:00}/{r.Anio}";
                ws.Cell(row, 2).Value = r.EmpleadoNombre;
                ws.Cell(row, 3).Value = r.SalarioBruto;
                ws.Cell(row, 4).Value = r.Deducciones;
                ws.Cell(row, 5).Value = r.Bonificaciones;
                ws.Cell(row, 6).Value = r.SalarioNeto;
                row++;
            }

            ws.Cell(row, 2).Value = "Totales";
            ws.Cell(row, 3).FormulaA1 = $"SUM(C2:C{row-1})";
            ws.Cell(row, 4).FormulaA1 = $"SUM(D2:D{row-1})";
            ws.Cell(row, 5).FormulaA1 = $"SUM(E2:E{row-1})";
            ws.Cell(row, 6).FormulaA1 = $"SUM(F2:F{row-1})";
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"HistorialPagos_{anioDesde}{mesDesde:00}_{anioHasta}{mesHasta:00}.xlsx";
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }

        public async Task<IActionResult> ExportPdf(int mesDesde, int anioDesde, int mesHasta, int anioHasta, int? empleadoId)
        {
            var result = await _service.GetAsync(mesDesde, anioDesde, mesHasta, anioHasta, empleadoId);

            QuestPDF.Settings.License = LicenseType.Community;
            var culture = new CultureInfo("es-CR");

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    var titulo = empleadoId.HasValue ? "Historial de Pagos (por empleado)" : "Historial de Pagos";
                    page.Header().Text($"{titulo} - {mesDesde:00}/{anioDesde} a {mesHasta:00}/{anioHasta}")
                        .SemiBold().FontSize(14).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(2); // Periodo
                            c.RelativeColumn(4); // Empleado
                            c.RelativeColumn(2); // Bruto
                            c.RelativeColumn(2); // Deducciones
                            c.RelativeColumn(2); // Bonificaciones
                            c.RelativeColumn(2); // Neto
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(Header).Text("Periodo");
                            h.Cell().Element(Header).Text("Empleado");
                            h.Cell().Element(Header).Text("Salario Bruto");
                            h.Cell().Element(Header).Text("Deducciones");
                            h.Cell().Element(Header).Text("Bonificaciones");
                            h.Cell().Element(Header).Text("Salario Neto");

                            static IContainer Header(IContainer c) => c.Background(Colors.Grey.Lighten3).Padding(4);
                        });

                        foreach (var r in result.Rows)
                        {
                            table.Cell().PaddingVertical(2).Text($"{r.Mes:00}/{r.Anio}");
                            table.Cell().PaddingVertical(2).Text(r.EmpleadoNombre);
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.SalarioBruto.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.Deducciones.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.Bonificaciones.ToString("C0", culture));
                            table.Cell().PaddingVertical(2).AlignRight().Text(r.SalarioNeto.ToString("C0", culture));
                        }

                        table.Cell().BorderTop(1).PaddingTop(4).Text("");
                        table.Cell().BorderTop(1).PaddingTop(4).Text("Totales");
                        table.Cell().BorderTop(1).PaddingTop(4).AlignRight().Text(result.TotalBruto.ToString("C0", culture));
                        table.Cell().BorderTop(1).PaddingTop(4).AlignRight().Text(result.TotalDeducciones.ToString("C0", culture));
                        table.Cell().BorderTop(1).PaddingTop(4).AlignRight().Text(result.TotalBonificaciones.ToString("C0", culture));
                        table.Cell().BorderTop(1).PaddingTop(4).AlignRight().Text(result.TotalNeto.ToString("C0", culture));
                    });

                    page.Footer().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            });

            var pdf = doc.GeneratePdf();
            var fileName = $"HistorialPagos_{anioDesde}{mesDesde:00}_{anioHasta}{mesHasta:00}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
