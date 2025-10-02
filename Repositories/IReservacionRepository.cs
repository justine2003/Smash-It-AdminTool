using SGA_Smash.Models;

namespace SGA_Smash.Repositories
{
    public interface IReservacionRepository
    {
        Task<IEnumerable<Reservacion>> GetAllReservaciones();
        Task<Reservacion> GetReservacionById(int id);
        Task AddReservacion(Reservacion reservacion);
        Task UpdateReservacion(Reservacion reservacion);
        Task DeleteReservacion(int id);
        Task<bool> ReservacionExists(int id);
    }
}