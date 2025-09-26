namespace SGA_Smash.Models;

public class Planilla
{
    public int Id { get; set; }
    public string EmpleadoNombre { get; set; }  // Simulado, no foreign key aún
    public decimal SalarioBase { get; set; }
    public decimal Deducciones { get; set; }
    public decimal Bonificaciones { get; set; }
    public decimal TotalPago => SalarioBase - Deducciones + Bonificaciones;
    public DateTime FechaPago { get; set; }
}