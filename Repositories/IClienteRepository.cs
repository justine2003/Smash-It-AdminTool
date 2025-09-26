// IClienteRepository.cs
using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllClientes();
        Task<Cliente> GetClienteById(int id);
        Task AddCliente(Cliente cliente);
        Task UpdateCliente(Cliente cliente);
        Task DeleteCliente(int id);
        Task<bool> ClienteExists(int id);
    }
}