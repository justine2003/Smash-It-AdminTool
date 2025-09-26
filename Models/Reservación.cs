namespace SGA_Smash.Models;

public class Reservacion
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; }
    public DateTime FechaHora { get; set; }
    public string Mesa { get; set; }
    public string Estado { get; set; }
    public string RegistradoPor { get; set; }
}