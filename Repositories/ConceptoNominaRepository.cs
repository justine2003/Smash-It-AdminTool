using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System.Text.Json;

namespace SGA_Smash.Repositories
{
    public class ConceptoNominaRepository : IConceptoNominaRepository
    {
        private readonly ApplicationDbContext _db;
        public ConceptoNominaRepository(ApplicationDbContext db) => _db = db;

        public async Task<(decimal deducciones, decimal bonificaciones)> GetTotalesActivosAsync(int empleadoId)
        {
            var activos = await _db.ConceptosNomina.AsNoTracking()
                .Where(c => c.EmpleadoId == empleadoId && c.Activo).ToListAsync();
            var ded = activos.Where(c => c.Tipo == "Deduccion").Sum(c => c.Monto);
            var bon = activos.Where(c => c.Tipo == "Bonificacion").Sum(c => c.Monto);
            return (ded, bon);
        }

        public async Task<IEnumerable<ConceptoNomina>> GetByEmpleadoAsync(int empleadoId) =>
            await _db.ConceptosNomina.AsNoTracking()
                .Where(c => c.EmpleadoId == empleadoId)
                .OrderBy(c => c.Tipo).ThenBy(c => c.Nombre)
                .ToListAsync();

        public Task<ConceptoNomina?> GetByIdAsync(int id) =>
            _db.ConceptosNomina.FirstOrDefaultAsync(c => c.Id == id);

        public async Task CreateAsync(ConceptoNomina c, string usuario)
        {
            await _db.ConceptosNomina.AddAsync(c);
            await _db.SaveChangesAsync();

            _db.ConceptoNominaLogs.Add(new ConceptoNominaLog
            {
                ConceptoId = c.Id,
                EmpleadoId = c.EmpleadoId,
                Usuario = usuario,
                Accion = "Create",
                ValorNuevo = JsonSerializer.Serialize(c)
            });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ConceptoNomina c, string usuario, ConceptoNomina before)
        {
            _db.Entry(c).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            _db.ConceptoNominaLogs.Add(new ConceptoNominaLog
            {
                ConceptoId = c.Id,
                EmpleadoId = c.EmpleadoId,
                Usuario = usuario,
                Accion = "Update",
                ValorAnterior = JsonSerializer.Serialize(before),
                ValorNuevo = JsonSerializer.Serialize(c)
            });
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string usuario)
        {
            var c = await _db.ConceptosNomina.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return;
            _db.ConceptosNomina.Remove(c);
            await _db.SaveChangesAsync();

            _db.ConceptoNominaLogs.Add(new ConceptoNominaLog
            {
                ConceptoId = id,
                EmpleadoId = c.EmpleadoId,
                Usuario = usuario,
                Accion = "Delete",
                ValorAnterior = System.Text.Json.JsonSerializer.Serialize(c)
            });
            await _db.SaveChangesAsync();
        }

        public async Task ToggleActivoAsync(int id, string usuario)
        {
            var c = await _db.ConceptosNomina.FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return;
            var before = JsonSerializer.Serialize(c);
            c.Activo = !c.Activo;
            await _db.SaveChangesAsync();

            _db.ConceptoNominaLogs.Add(new ConceptoNominaLog
            {
                ConceptoId = id,
                EmpleadoId = c.EmpleadoId,
                Usuario = usuario,
                Accion = "ToggleActivo",
                ValorAnterior = before,
                ValorNuevo = JsonSerializer.Serialize(c)
            });
            await _db.SaveChangesAsync();
        }
    }
}
