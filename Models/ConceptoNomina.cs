using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("ConceptoNomina")]
    public class ConceptoNomina
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("empleado_id")]
        public int EmpleadoId { get; set; }

        // "Deduccion" | "Bonificacion"
        [Required, StringLength(20), Column("tipo")]
        public string Tipo { get; set; } = "Deduccion";

        [Required, StringLength(100), Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue), Column("monto")]
        public decimal Monto { get; set; }

        [Required, Column("activo")]
        public bool Activo { get; set; } = true;

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }
    }
}
