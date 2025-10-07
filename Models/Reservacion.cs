using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    public class Reservacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Column("cliente_id")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }



        [Required(ErrorMessage = "Debe ingresar fecha y hora")]
        [DataType(DataType.DateTime)]
        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Column("mesa")]
        [Required(ErrorMessage = "Debe ingresar la mesa")]
        public string Mesa { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("registrado_por")]
        public int? RegistradoPor { get; set; }

    }
}
