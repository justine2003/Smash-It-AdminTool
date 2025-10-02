namespace SGA_Smash.Models;

public class Inventario
{
    public int Id { get; set; }
    public string ProductoNombre { get; set; } 
    public string ProveedorNombre { get; set; } 
    public int Cantidad { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Estado { get; set; }
}