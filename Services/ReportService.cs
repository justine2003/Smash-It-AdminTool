using SGA_Smash.Models;
using SGA_Smash.Repositories;

namespace SGA_Smash.Services
{
    public class ReportService
    {
        private readonly IPlanillaRepository _planillas;
        public ReportService(IPlanillaRepository planillas) => _planillas = planillas;

        public async Task<(IEnumerable<Planilla> items, decimal bruto, decimal ded, decimal bon, decimal neto)>
            GetMensualAsync(int mes, int anio)
        {
            var items = await _planillas.GetByMesAnioAsync(mes, anio);
            var bruto = items.Sum(x => x.SalarioBase);
            var ded   = items.Sum(x => x.Deducciones);
            var bon   = items.Sum(x => x.Bonificaciones);
            var neto  = items.Sum(x => x.SalarioNeto);
            return (items, bruto, ded, bon, neto);
        }
    }
}
