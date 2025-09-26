namespace SGA_Smash.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string UnidadMedida { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PrecioEntregaDias { get; set; }
    public int StockActual { get; set; }
    public int MinimoStock { get; set; }
}