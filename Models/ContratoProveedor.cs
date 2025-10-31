using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("ContratoProveedor")]
    public class ContratoProveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Proveedor")]
        [Column("proveedor_id")]
        public int ProveedorId { get; set; }

        public Proveedor? Proveedor { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("monto_total")]
        public decimal MontoTotal { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("ruta_archivo")]
        public string? RutaArchivo { get; set; }

        // Deshabilitado: detalles de productos por contrato
        // public ICollection<ContratoProducto>? ContratoProductos { get; set; }
    }

}
