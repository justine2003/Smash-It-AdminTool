using System;
using System.Collections.Generic;
using System.Linq;

namespace SGA_Smash.Models.Reporting
{
    public class PaymentHistoryRow
    {
        public int EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;

        public int Mes { get; set; }
        public int Anio { get; set; }

        public decimal SalarioBruto { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Bonificaciones { get; set; }
        public decimal SalarioNeto => SalarioBruto - Deducciones + Bonificaciones;

        // Útil para ordenar por “fecha” (usa día 1 del mes)
        public DateTime Periodo => new DateTime(Anio, Mes, 1);
    }

    public class PaymentHistoryResult
    {
        public int MesDesde { get; set; }
        public int AnioDesde { get; set; }
        public int MesHasta { get; set; }
        public int AnioHasta { get; set; }
        public int? EmpleadoId { get; set; }

        public List<PaymentHistoryRow> Rows { get; set; } = new();

        public decimal TotalBruto => Rows.Sum(r => r.SalarioBruto);
        public decimal TotalDeducciones => Rows.Sum(r => r.Deducciones);
        public decimal TotalBonificaciones => Rows.Sum(r => r.Bonificaciones);
        public decimal TotalNeto => Rows.Sum(r => r.SalarioNeto);
    }
}
