using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models
{
    [Table("Proveedor")]
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
        [Column("contacto")]
        public string Contacto { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Column("telefono")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [Column("correo")]
        public string Correo { get; set; }
    }
}