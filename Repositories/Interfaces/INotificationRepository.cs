using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetOpsNotificationAsync(bool? isRead);
    Task<List<Notification>> GetAdminNotificationAsync(int adminId, bool? isRead);
    Task<int> GetUnreadCountForOpsAsync();
    Task<int> GetUnreadCountForAdminAsync(int adminId);
    Task MarkOpsAsReadAsync();
    Task MarkAdminAsReadAsync(int adminId);
}