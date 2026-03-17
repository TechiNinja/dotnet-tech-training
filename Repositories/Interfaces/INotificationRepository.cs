using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<NotificationResponseDto>> GetOpsAsync();
    Task<List<NotificationResponseDto>> GetAdminAsync(int adminId);
    Task<int> GetUnreadCountForOpsAsync();
    Task<int> GetUnreadCountForAdminAsync(int adminId);
    Task MarkOpsAsReadAsync();
    Task MarkAdminAsReadAsync(int adminId);
}