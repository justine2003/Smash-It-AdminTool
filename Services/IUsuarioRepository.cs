using SGA_Smash.Models;

namespace SGA_Smash.Services
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByNombreAsync(string nombre);
        Task<bool> ExistsByNombreAsync(string nombre);
        Task AddAsync(Usuario usuario);
        Task SaveChangesAsync();
    }
}
