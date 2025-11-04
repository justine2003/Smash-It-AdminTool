using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("Planilla")]
    public class Planilla
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required, Range(1,12), Column("mes")]
        public int Mes { get; set; }

        [Required, Range(2000,2100), Column("anio")]
        public int Anio { get; set; }

        [Required, Range(0, double.MaxValue), Column("salario_base")]
        public decimal SalarioBase { get; set; }

        [Required, Range(0, double.MaxValue), Column("bonificaciones")]
        public decimal Bonificaciones { get; set; }

        [Required, Range(0, double.MaxValue), Column("deducciones")]
        public decimal Deducciones { get; set; }

        [NotMapped]
        public decimal SalarioNeto => SalarioBase - Deducciones + Bonificaciones;

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }
    }
}
