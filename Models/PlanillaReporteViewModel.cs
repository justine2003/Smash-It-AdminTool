using System.Collections.Generic;

namespace SGA_Smash.Models.ViewModels
{
    public class PlanillaReporteViewModel
    {
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public IEnumerable<Planilla> Planillas { get; set; }
    }
}
