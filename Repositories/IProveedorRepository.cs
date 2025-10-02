using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<Proveedor>> GetAllProveedores();
        Task<Proveedor> GetProveedorById(int id);
        Task AddProveedor(Proveedor proveedor);
        Task UpdateProveedor(Proveedor proveedor);
        Task DeleteProveedor(int id);
        Task<bool> ProveedorExists(int id);
    }
}
