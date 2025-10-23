using System.Threading.Tasks;

namespace SGA_Smash.Services
{
    public interface INotificationService
    {
        Task NotifyAdminsAsync(string subject, string body);
    }
}
