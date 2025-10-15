using System.Threading.Tasks;
using SGA_Smash.Models.Reporting;

namespace SGA_Smash.Repositories
{
    public interface IPlanillaReportService
    {
        Task<PayrollReportResult> GetMonthlyAsync(int mes, int anio);
    }
}
