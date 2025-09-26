public class Notificacion
{
    public int Id { get; set; }
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } // "Alerta", "Info", "Error", etc.
}