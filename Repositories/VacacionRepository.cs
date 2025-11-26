using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA_Smash.Repositories
{
    public class VacacionRepository : IVacacionRepository
    {
        private readonly ApplicationDbContext _context;

        public VacacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vacacion>> GetAllAsync()
        {
            return await _context.Vacaciones.ToListAsync();
        }

        public async Task<Vacacion?> GetByIdAsync(int id)
        {
            return await _context.Vacaciones.FindAsync(id);
        }

        public async Task AddAsync(Vacacion v)
        {
            _context.Vacaciones.Add(v);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vacacion v)
        {
            _context.Entry(v).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Vacaciones.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var vacacion = await _context.Vacaciones.FindAsync(id);
            if (vacacion != null)
            {
                _context.Vacaciones.Remove(vacacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Vacacion>> GetByEmpleadoAsync(int empleadoId)
        {
            return await _context.Vacaciones
                .Where(v => v.EmpleadoId == empleadoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vacacion>> GetPendientesAsync()
        {
            return await _context.Vacaciones
                .Where(v => v.Estado == "Pendiente")
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(int empleadoId, DateTime inicio, DateTime fin)
        {
            return await _context.Vacaciones
                .AnyAsync(v => v.EmpleadoId == empleadoId &&
                              v.Estado != "Rechazada" &&
                              v.FechaInicio < fin &&
                              v.FechaFin > inicio);
        }

        public async Task ApproveAsync(Vacacion v, int aprobadorId)
        {
            v.Estado = "Aprobada";
            v.AprobadoPor = aprobadorId;
            _context.Entry(v).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RejectAsync(Vacacion v, int aprobadorId)
        {
            v.Estado = "Rechazada";
            v.AprobadoPor = aprobadorId;
            _context.Entry(v).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
