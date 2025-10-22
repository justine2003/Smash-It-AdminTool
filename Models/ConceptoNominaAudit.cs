using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("ConceptoNominaAudit")]
    public class ConceptoNominaAudit
    {
        [Key, Column("id")] public int Id { get; set; }

        [Required, Column("concepto_id")] public int ConceptoId { get; set; }

        [Required, Column("fecha")] public DateTime Fecha { get; set; }

        [Required, StringLength(256), Column("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Required, Column("accion")] public string Accion { get; set; } = "UPDATE";

        [Column("valor_anterior", TypeName = "decimal(10,2)")] public decimal ValorAnterior { get; set; }

        [Column("valor_nuevo", TypeName = "decimal(10,2)")] public decimal ValorNuevo { get; set; }

        [ForeignKey(nameof(ConceptoId))] public ConceptoNomina? Concepto { get; set; }
    }
}
