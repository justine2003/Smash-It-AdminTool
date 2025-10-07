using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA_Smash.Models;

    [Table("Vacacion")]
    public class Vacacion : IValidatableObject
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required, DataType(DataType.Date), Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required, DataType(DataType.Date), Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Required, StringLength(50), Column("estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente | Aprobada | Rechazada

        [Required, Column("dias_solicitados")]
        public int DiasSolicitados { get; set; }

        [Required, DataType(DataType.Date), Column("fecha_solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Today;

        [Column("aprobado_por")]
        public int? AprobadoPor { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }

        [ForeignKey(nameof(AprobadoPor))]
        public Empleado? Aprobador { get; set; }

        [NotMapped]
        public int DiasCalculados => (int)(FechaFin.Date - FechaInicio.Date).TotalDays + 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFin.Date < FechaInicio.Date)
                yield return new ValidationResult("La fecha fin no puede ser anterior a la fecha de inicio.", new[] { nameof(FechaFin) });

            if (DiasSolicitados <= 0)
                yield return new ValidationResult("Los días solicitados deben ser mayores a cero.", new[] { nameof(DiasSolicitados) });

            if (Estado == "Aprobada" && AprobadoPor == null)
                yield return new ValidationResult("Debe indicar quién aprobó la solicitud.", new[] { nameof(AprobadoPor) });
        }
    }


