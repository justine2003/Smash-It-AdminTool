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
        Task<IEnumerable<HorarioDemanda>> GetHorariosConMasDemanda(DateTime? desde, DateTime? hasta, int top);
        Task<IEnumerable<Reservacion>> GetReservacionesConfirmadasYCanceladas(DateTime? desde, DateTime? hasta);
        Task<List<MesaDisponible>> GetMesasDisponibles(DateTime fechaHora, int numeroPersonas, int? reservacionIdExcluir = null);
    }
}