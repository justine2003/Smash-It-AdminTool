using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using SGA_Smash.Models.Reporting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SGA_Smash.Repositories
{
    public class HistorialPagosService : IHistorialPagosService
    {
        private readonly ApplicationDbContext _db;

        public HistorialPagosService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaymentHistoryResult> GetAsync(int mesDesde, int anioDesde, int mesHasta, int anioHasta, int? empleadoId)
        {
            // Convertimos el rango mes/año a una tupla comparable (anio, mes)
            (int y, int m) inicio = (anioDesde, mesDesde);
            (int y, int m) fin    = (anioHasta, mesHasta);

            // Consulta base
            var q = from p in _db.Set<Planilla>().AsNoTracking()
                    where (p.Anio > inicio.y || (p.Anio == inicio.y && p.Mes >= inicio.m))
                      && (p.Anio < fin.y    || (p.Anio == fin.y    && p.Mes <= fin.m))
                    select p;

            if (empleadoId.HasValue)
                q = q.Where(p => p.EmpleadoId == empleadoId.Value);

            var qJoin = from p in q
                        join e in _db.Set<Empleado>().AsNoTracking() on p.EmpleadoId equals e.Id into je
                        from e in je.DefaultIfEmpty()
                        select new PaymentHistoryRow
                        {
                            EmpleadoId = p.EmpleadoId,
                            EmpleadoNombre = e != null ? e.Nombre : ("Empleado #" + p.EmpleadoId),
                            Mes = p.Mes,
                            Anio = p.Anio,
                            SalarioBruto = p.SalarioBase,
                            Deducciones = p.Deducciones,
                            Bonificaciones = p.Bonificaciones
                        };

            var rows = await qJoin
                .OrderByDescending(r => r.Anio)
                .ThenByDescending(r => r.Mes) // orden por “fecha” descendente
                .ThenBy(r => r.EmpleadoNombre)
                .ToListAsync();

            return new PaymentHistoryResult
            {
                MesDesde  = mesDesde,
                AnioDesde = anioDesde,
                MesHasta  = mesHasta,
                AnioHasta = anioHasta,
                EmpleadoId = empleadoId,
                Rows = rows
            };
        }
    }
}
