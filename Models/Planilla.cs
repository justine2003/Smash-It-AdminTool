using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("Planilla")]
    public class Planilla
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio.")]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
        [Column("mes")]
        public int Mes { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100.")]
        [Column("anio")]
        public int Anio { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El salario base debe ser un número válido y no negativo.")]
        [Column("salario_base", TypeName = "decimal(10,2)")]
        public decimal SalarioBase { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Las bonificaciones deben ser un número válido y no negativo.")]
        [Column("bonificaciones", TypeName = "decimal(10,2)")]
        public decimal Bonificaciones { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Las deducciones deben ser un número válido y no negativo.")]
        [Column("deducciones", TypeName = "decimal(10,2)")]
        public decimal Deducciones { get; set; }

        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal TotalPago => SalarioBase - Deducciones + Bonificaciones;

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }
    }
}
