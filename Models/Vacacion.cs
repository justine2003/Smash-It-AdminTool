namespace SGA_Smash.Models;

public class Vacacion
{
    public int Id { get; set; }
    public string EmpleadoNombre { get; set; } 
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int Dias => (FechaFin - FechaInicio).Days + 1;
    public string Estado { get; set; } 
}