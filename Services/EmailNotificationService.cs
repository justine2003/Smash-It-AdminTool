using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SGA_Smash.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IConfiguration _cfg;
        public EmailNotificationService(IConfiguration cfg) => _cfg = cfg;

        public Task NotifyAdminsAsync(string subject, string body)
        {
            // En producción: envía correo o integra tu canal de notificaciones
            System.Diagnostics.Debug.WriteLine($"[ADMIN NOTIFY] {subject}\n{body}");
            return Task.CompletedTask;
        }
    }
}
