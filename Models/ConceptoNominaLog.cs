using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("ConceptoNominaLog")]
    public class ConceptoNominaLog
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Column("concepto_id")]
        public int? ConceptoId { get; set; }

        [Required, Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required, StringLength(100), Column("usuario")]
        public string Usuario { get; set; } = string.Empty;

        // Create | Update | Delete | ToggleActivo
        [Required, StringLength(20), Column("accion")]
        public string Accion { get; set; } = "Update";

        [Column("valor_anterior")]
        public string? ValorAnterior { get; set; }

        [Column("valor_nuevo")]
        public string? ValorNuevo { get; set; }

        [Required, Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
