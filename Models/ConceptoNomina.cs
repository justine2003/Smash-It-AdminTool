using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("ConceptoNomina")]
    public class ConceptoNomina
    {
        [Key, Column("id")] public int Id { get; set; }

        [Required, Column("empleado_id")] public int EmpleadoId { get; set; }

        [Required, StringLength(120), Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required, Column("tipo")] public char Tipo { get; set; } = 'D'; // 'D' o 'B'

        [Required, Column("monto", TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto debe ser >= 0")]
        public decimal Monto { get; set; }

        [Required, Column("activo")] public bool Activo { get; set; } = true;

        [Column("vigente_desde")] public DateTime? VigenteDesde { get; set; }

        [ForeignKey(nameof(EmpleadoId))] public Empleado? Empleado { get; set; }
    }
}
