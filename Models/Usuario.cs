using System;
using System.ComponentModel.DataAnnotations;

namespace SGA_Smash.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string nombre { get; set; }

        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [Display(Name = "Correo electrónico")]
        public string? correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string contrasena { get; set; }

        public DateTime? fecha_creacion { get; set; }
        public DateTime? ultimo_acceso { get; set; }
        public int? rol_id { get; set; }
    }
}

