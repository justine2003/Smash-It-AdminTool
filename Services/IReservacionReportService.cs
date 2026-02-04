namespace SGA_Smash.Services;

public interface IReservacionReportService
{
    Task<byte[]> GenerateReservacionesPDF(DateTime? desde = null, DateTime? hasta = null);
}


