using System.Threading.Tasks;
using SGA_Smash.Models.Reporting;

namespace SGA_Smash.Repositories
{
    public interface IHistorialPagosService
    {
        Task<PaymentHistoryResult> GetAsync(int mesDesde, int anioDesde, int mesHasta, int anioHasta, int? empleadoId);
    }
}
