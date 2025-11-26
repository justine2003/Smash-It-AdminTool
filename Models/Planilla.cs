using System;
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

        [Required]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required]
        [Column("mes")]
        public int Mes { get; set; }   // 1-12

        [Required]
        [Column("anio")]
        public int Anio { get; set; }

        [Required]
        [Column("salario_base", TypeName = "decimal(10,2)")]
        [Display(Name = "Salario base")]
        public decimal SalarioBase { get; set; }

        [Required]
        [Column("bonificaciones", TypeName = "decimal(10,2)")]
        [Display(Name = "Bonificaciones")]
        public decimal Bonificaciones { get; set; }

        [Required]
        [Column("deducciones", TypeName = "decimal(10,2)")]
        [Display(Name = "Deducciones")]
        public decimal Deducciones { get; set; }

        [Required]
        [Column("salario_neto", TypeName = "decimal(10,2)")]
        [Display(Name = "Salario neto")]
        public decimal SalarioNeto { get; set; }

        [Required]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }

        [NotMapped]
        [Display(Name = "Salario bruto")]
        public decimal SalarioBruto => SalarioBase + Bonificaciones;

        // Para compatibilidad con tu Index actual
        [NotMapped]
        public decimal TotalPago => SalarioNeto;
    }
}
