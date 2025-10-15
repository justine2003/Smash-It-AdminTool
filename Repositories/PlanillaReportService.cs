using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using SGA_Smash.Models.Reporting;
using System.Linq;
using System.Threading.Tasks;

namespace SGA_Smash.Repositories
{
    public class PlanillaReportService : IPlanillaReportService
    {
        private readonly ApplicationDbContext _db;

        public PlanillaReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PayrollReportResult> GetMonthlyAsync(int mes, int anio)
        {
            var q = from p in _db.Set<Planilla>().AsNoTracking().Where(p => p.Mes == mes && p.Anio == anio)
                    join e in _db.Set<Empleado>().AsNoTracking() on p.EmpleadoId equals e.Id into je
                    from e in je.DefaultIfEmpty()
                    select new PayrollReportRow
                    {
                        EmpleadoId = p.EmpleadoId,
                        EmpleadoNombre = e != null ? e.Nombre : ("Empleado #" + p.EmpleadoId),
                        SalarioBruto = p.SalarioBase,
                        Deducciones = p.Deducciones,
                        Bonificaciones = p.Bonificaciones
                    };

            var rows = await q.OrderBy(r => r.EmpleadoNombre).ToListAsync();

            return new PayrollReportResult
            {
                Mes = mes,
                Anio = anio,
                Rows = rows
            };
        }
    }
}
