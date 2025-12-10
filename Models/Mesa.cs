using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    public class Mesa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("numero")]
        [MaxLength(10)]
        public string Numero { get; set; }

        [Required]
        [Column("capacidad")]
        [Range(1, 20)]
        public int Capacidad { get; set; }

        [Column("estado")]
        [MaxLength(20)]
        public string Estado { get; set; } = "Disponible";
    }
}

