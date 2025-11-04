using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("Empleado")]
    public class Empleado
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, StringLength(120), Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(40), Column("identificacion")]
        public string? Identificacion { get; set; }

        [Required, Column("dias_vacaciones_disponibles")]
        public int DiasVacacionesDisponibles { get; set; } = 0;

        [Column("fecha_ingreso")]
        public DateTime? FechaIngreso { get; set; }

        [Required, Column("activo")]
        public bool Activo { get; set; } = true;
    }
}
