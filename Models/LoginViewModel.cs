namespace SGA_Smash.Models
{
    public class LoginViewModel
    {
        public string nombre { get; set; }
        public string contrasena { get; set; }
        public int? rol_id { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public DateTime? ultimo_acceso{ get; set; }
    }
}