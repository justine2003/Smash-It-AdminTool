using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("Empleado")]
    public class Empleado
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Puesto")]
        public string Puesto { get; set; }

        [Column("SalarioBase", TypeName = "decimal(10,2)")]
        public decimal? SalarioBase { get; set; }

        [Column("FechaIngreso")]
        public DateTime? FechaIngreso { get; set; }

        [Column("Estado")]
        public string Estado { get; set; }

        // Para próxima planilla
        [Required]
        [Column("deducciones_fijas", TypeName = "decimal(10,2)")]
        [Display(Name = "Deducciones fijas")]
        public decimal DeduccionesFijas { get; set; }

        [Required]
        [Column("bonificaciones_fijas", TypeName = "decimal(10,2)")]
        [Display(Name = "Bonificaciones fijas")]
        public decimal BonificacionesFijas { get; set; }

        [Column("dias_vacaciones_disponibles")]
        public int DiasVacacionesDisponibles { get; set; } = 0;
    }
}
