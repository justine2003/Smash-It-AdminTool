namespace SGA_Smash.Models;

public class Gasto
{
    public int id { get; set; }
    public decimal monto { get; set; }
    public DateTime fecha { get; set; }
    public string tipo { get; set; }
    public string descripcion { get; set; }
    public int registrado_por { get; set; }

    public Empleado? RegistroEmpleado { get; set; }
}