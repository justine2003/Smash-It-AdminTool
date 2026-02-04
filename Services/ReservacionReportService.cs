using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SGA_Smash.Models;
using SGA_Smash.Repositories;
using System.Globalization;

namespace SGA_Smash.Services
{
    public class ReservacionReportService : IReservacionReportService
    {
        private readonly IReservacionRepository _reservacionRepository;

        public ReservacionReportService(IReservacionRepository reservacionRepository)
        {
            _reservacionRepository = reservacionRepository;
        }

        public async Task<byte[]> GenerateReservacionesPDF(DateTime? desde = null, DateTime? hasta = null)
        {
            // Configurar QuestPDF ANTES del try-catch para evitar problemas de inicialización
            // Esto debe hacerse justo antes de usar Document.Create, igual que otros controladores
            QuestPDF.Settings.License = LicenseType.Community;
            
            try
            {
                var culture = new CultureInfo("es-ES");

                // Usar el método específico para obtener reservaciones confirmadas y canceladas
                var reservaciones = await _reservacionRepository.GetReservacionesConfirmadasYCanceladas(desde, hasta);

                var lista = reservaciones?.ToList() ?? new List<Reservacion>();

                var confirmadas = lista.Where(r => r.Estado == "Confirmada").ToList();
                var canceladas = lista.Where(r => r.Estado == "Cancelada").ToList();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // ENCABEZADO
                    page.Header().Column(col =>
                    {
                        col.Item().Text("Reporte de Reservaciones")
                            .FontSize(16).SemiBold().AlignCenter();

                        var fechaRango = desde.HasValue && hasta.HasValue
                            ? $"Del {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy}"
                            : desde.HasValue ? $"Desde {desde.Value:dd/MM/yyyy}"
                            : hasta.HasValue ? $"Hasta {hasta.Value:dd/MM/yyyy}"
                            : "Todas las reservaciones";

                        col.Item().PaddingTop(5).Text(fechaRango).FontSize(12).AlignCenter();

                        col.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Confirmadas: {confirmadas.Count}");
                            row.RelativeItem().Text($"Canceladas: {canceladas.Count}");
                        });
                    });

                    // CONTENIDO
                    page.Content().Column(col =>
                    {
                        // Confirmadas
                        if (confirmadas.Any())
                        {
                            col.Item().PaddingTop(10)
                                .Text("RESERVACIONES CONFIRMADAS")
                                .FontSize(12).SemiBold().FontColor(Colors.Green.Darken2);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.ConstantColumn(40);
                                    cols.RelativeColumn(3);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(HeaderGreen).Text("ID");
                                    h.Cell().Element(HeaderGreen).Text("Cliente");
                                    h.Cell().Element(HeaderGreen).Text("Fecha/Hora");
                                    h.Cell().Element(HeaderGreen).Text("Mesa");
                                    h.Cell().Element(HeaderGreen).Text("Personas");
                                    h.Cell().Element(HeaderGreen).Text("Estado");
                                });

                                foreach (var r in confirmadas)
                                {
                                    table.Cell().Padding(3).Border(1).Text(r.Id.ToString());
                                    table.Cell().Padding(3).Border(1).Text(r.Cliente?.Nombre ?? "N/A");
                                    table.Cell().Padding(3).Border(1).Text(r.FechaHora.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().Padding(3).Border(1).Text(r.Mesa);
                                    table.Cell().Padding(3).Border(1).Text(r.NumeroPersonas.ToString());
                                    table.Cell().Padding(3).Border(1).Text(r.Estado);
                                }
                            });
                        }

                        // Canceladas
                        if (canceladas.Any())
                        {
                            col.Item().PaddingTop(15)
                                .Text("RESERVACIONES CANCELADAS")
                                .FontSize(12).SemiBold().FontColor(Colors.Red.Darken2);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.ConstantColumn(40);
                                    cols.RelativeColumn(3);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(2);
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Element(HeaderRed).Text("ID");
                                    h.Cell().Element(HeaderRed).Text("Cliente");
                                    h.Cell().Element(HeaderRed).Text("Fecha/Hora");
                                    h.Cell().Element(HeaderRed).Text("Mesa");
                                    h.Cell().Element(HeaderRed).Text("Personas");
                                    h.Cell().Element(HeaderRed).Text("Estado");
                                });

                                foreach (var r in canceladas)
                                {
                                    table.Cell().Padding(3).Border(1).Text(r.Id.ToString());
                                    table.Cell().Padding(3).Border(1).Text(r.Cliente?.Nombre ?? "N/A");
                                    table.Cell().Padding(3).Border(1).Text(r.FechaHora.ToString("dd/MM/yyyy HH:mm"));
                                    table.Cell().Padding(3).Border(1).Text(r.Mesa);
                                    table.Cell().Padding(3).Border(1).Text(r.NumeroPersonas.ToString());
                                    table.Cell().Padding(3).Border(1).Text(r.Estado);
                                }
                            });
                        }

                        // Mensaje si no hay datos
                        if (!confirmadas.Any() && !canceladas.Any())
                        {
                            col.Item().PaddingTop(20)
                                .Text("No se encontraron reservaciones confirmadas o canceladas en el rango de fechas especificado.")
                                .FontSize(11)
                                .Italic()
                                .FontColor(Colors.Grey.Medium)
                                .AlignCenter();
                        }
                    });

                    // PIE
                    page.Footer().AlignRight().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            return doc.GeneratePdf();
            }
            catch (Exception ex)
            {
                // Log del error (si tienes un logger, úsalo aquí)
                throw new Exception($"Error al generar el PDF de reservaciones: {ex.Message}", ex);
            }
        }

        // Helpers
        private static IContainer HeaderGreen(IContainer c) =>
            c.Background(Colors.Green.Lighten4).Padding(4).Border(1);

        private static IContainer HeaderRed(IContainer c) =>
            c.Background(Colors.Red.Lighten4).Padding(4).Border(1);

    }
}
