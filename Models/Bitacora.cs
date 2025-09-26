namespace SGA_Smash.Models;

public class Bitacora
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Modulo { get; set; }
    public string Accion { get; set; }
    public DateTime Fecha { get; set; }
    public string Detalles { get; set; }
}