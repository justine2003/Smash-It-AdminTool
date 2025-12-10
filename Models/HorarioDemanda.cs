namespace SGA_Smash.Models
{
    public class HorarioDemanda
    {
        public int Hora { get; set; }
        public int CantidadReservaciones { get; set; }
        public string HoraFormato => $"{Hora:00}:00";
    }
}
