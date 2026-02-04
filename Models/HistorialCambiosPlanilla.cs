using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("HistorialCambiosPlanilla")]
    public class HistorialCambiosPlanilla
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("campo_modificado")]
        public string CampoModificado { get; set; }  // "deducciones_fijas" o "bonificaciones_fijas"

        [Required]
        [Column("valor_anterior", TypeName = "decimal(10,2)")]
        public decimal ValorAnterior { get; set; }

        [Required]
        [Column("valor_nuevo", TypeName = "decimal(10,2)")]
        public decimal ValorNuevo { get; set; }

        [Required]
        [Column("fecha_cambio")]
        public DateTime FechaCambio { get; set; }

        // opcional: nav props
        public Empleado Empleado { get; set; }
        public Usuario Usuario { get; set; }
    }
}
