using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetOpsAsync();
    Task<List<Notification>> GetAdminAsync(int adminId);
}