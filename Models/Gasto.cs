namespace SGA_Smash.Models;

public class Gasto
{
    public int Id { get; set; }
    public string Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public string RegistradoPor { get; set; }
}