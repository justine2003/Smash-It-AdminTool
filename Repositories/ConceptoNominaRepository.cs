using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class ConceptoNominaRepository : IConceptoNominaRepository
    {
        private readonly ApplicationDbContext _db;
        public ConceptoNominaRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<ConceptoNomina>> GetActivosByEmpleadoAsync(int empleadoId)
        {
            return await _db.Set<ConceptoNomina>()
                .AsNoTracking()
                .Where(c => c.EmpleadoId == empleadoId && c.Activo)
                .OrderBy(c => c.Tipo).ThenBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<ConceptoNomina?> GetByIdAsync(int id)
        {
            return await _db.Set<ConceptoNomina>().FindAsync(id);
        }

        public async Task UpdateAsync(ConceptoNomina concepto, string usuario)
        {
            var original = await _db.Set<ConceptoNomina>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == concepto.Id);
            if (original == null) throw new InvalidOperationException("Concepto no encontrado.");
            if (!original.Activo) throw new InvalidOperationException("Solo se pueden modificar conceptos activos.");

            // Audit
            _db.Set<ConceptoNominaAudit>().Add(new ConceptoNominaAudit
            {
                ConceptoId = concepto.Id,
                Fecha = DateTime.Now,
                Usuario = string.IsNullOrWhiteSpace(usuario) ? "sistema" : usuario,
                Accion = "UPDATE",
                ValorAnterior = original.Monto,
                ValorNuevo = concepto.Monto
            });

            _db.Entry(concepto).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<(decimal totalDeducciones, decimal totalBonificaciones)> GetTotalesActivosAsync(int empleadoId)
        {
            var activos = await _db.Set<ConceptoNomina>()
                .AsNoTracking()
                .Where(c => c.EmpleadoId == empleadoId && c.Activo)
                .ToListAsync();

            var ded = activos.Where(c => c.Tipo == 'D').Sum(c => c.Monto);
            var bon = activos.Where(c => c.Tipo == 'B').Sum(c => c.Monto);
            return (ded, bon);
        }
    }
}
