using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateAsync(
            int? userId,
            int requestId,
            string message,
            NotificationType type,
            NotificationAudience audience
        );

        Task<List<Notification>> GetOpsAsync();

        Task<List<Notification>> GetAdminAsync(int adminId);
    }
}