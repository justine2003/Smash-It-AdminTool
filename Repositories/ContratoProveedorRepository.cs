using Microsoft.EntityFrameworkCore;
using SGA_Smash.Models;
using SGA_Smash.Data;

namespace SGA_Smash.Repositories
{
    public class ContratoProveedorRepository : IContratoProveedorRepository
    {
        private readonly ApplicationDbContext _context;
        public ContratoProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ContratoProveedor>> GetAllContratoProveedoresAsync()
        {
            return await _context.ContratoProveedores
                .Include(c => c.Proveedor)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();
        }

        public async Task<ContratoProveedor?> GetContratoProveedorByIdAsync(int id)
        {
            return await _context.ContratoProveedores.FindAsync(id);
        }

        public async Task<ContratoProveedor?> GetContratoProveedorWithProveedorAsync(int id)
        {
            return await _context.ContratoProveedores
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<ContratoProveedor>> GetContratosByProveedorIdAsync(int proveedorId)
        {
            return await _context.ContratoProveedores
                .Include(c => c.Proveedor)
                .Where(c => c.ProveedorId == proveedorId)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();
        }

        public async Task AddContratoProveedorAsync(ContratoProveedor contratoProveedor)
        {
            _context.ContratoProveedores.Add(contratoProveedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContratoProveedorAsync(ContratoProveedor contratoProveedor)
        {
            _context.Entry(contratoProveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContratoProveedorAsync(int id)
        {
            var contratoProveedor = await _context.ContratoProveedores.FindAsync(id);
            if (contratoProveedor != null)
            {
                _context.ContratoProveedores.Remove(contratoProveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ContratoProveedorExistsAsync(int id)
        {
            return await _context.ContratoProveedores.AnyAsync(e => e.Id == id);
        }
    }
}
