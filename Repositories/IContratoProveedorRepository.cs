using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IContratoProveedorRepository
    {
        Task<IEnumerable<ContratoProveedor>> GetAllContratoProveedoresAsync();
        Task<ContratoProveedor?> GetContratoProveedorByIdAsync(int id);
        Task<ContratoProveedor?> GetContratoProveedorWithProveedorAsync(int id);
        Task<IEnumerable<ContratoProveedor>> GetContratosByProveedorIdAsync(int proveedorId);
        Task AddContratoProveedorAsync(ContratoProveedor contratoProveedor);
        Task UpdateContratoProveedorAsync(ContratoProveedor contratoProveedor);
        Task DeleteContratoProveedorAsync(int id);
        Task<bool> ContratoProveedorExistsAsync(int id);
    }
}