using Microsoft.EntityFrameworkCore;
using SGA_Smash.Data;
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proveedor>> GetAllProveedores()
        {
            return await _context.Proveedores.ToListAsync();
        }

        public async Task<Proveedor> GetProveedorById(int id)
        {
            return await _context.Proveedores.FindAsync(id);
        }

        public async Task AddProveedor(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProveedor(Proveedor proveedor)
        {
            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProveedorExists(int id)
        {
            return await _context.Proveedores.AnyAsync(e => e.Id == id);
        }
    }
}
