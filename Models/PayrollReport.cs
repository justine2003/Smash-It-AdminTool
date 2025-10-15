using System;
using System.Collections.Generic;
using System.Linq;

namespace SGA_Smash.Models.Reporting
{
    public class PayrollReportRow
    {
        public int EmpleadoId { get; set; }
        public string EmpleadoNombre { get; set; } = string.Empty;
        public decimal SalarioBruto { get; set; }
        public decimal Deducciones { get; set; }
        public decimal Bonificaciones { get; set; }
        public decimal SalarioNeto => SalarioBruto - Deducciones + Bonificaciones;
    }

    public class PayrollReportResult
    {
        public int Mes { get; set; }
        public int Anio { get; set; }
        public List<PayrollReportRow> Rows { get; set; } = new();
        public decimal TotalBruto => Rows.Sum(r => r.SalarioBruto);
        public decimal TotalDeducciones => Rows.Sum(r => r.Deducciones);
        public decimal TotalBonificaciones => Rows.Sum(r => r.Bonificaciones);
        public decimal TotalNeto => Rows.Sum(r => r.SalarioNeto);
    }
}
