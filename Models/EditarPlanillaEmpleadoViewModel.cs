namespace SGA_Smash.Models.ViewModels
{
    public class EditarPlanillaEmpleadoViewModel
    {
        public int EmpleadoId { get; set; }
        public string Nombre { get; set; }
        public decimal? SalarioBase { get; set; }

        public decimal DeduccionesFijas { get; set; }
        public decimal BonificacionesFijas { get; set; }
    }
}
