using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateAsync(BaseNotificationDto dto);
    Task<List<NotificationResponseDto>> GetOpsNotificationAsync(bool? isRead);
    Task<List<NotificationResponseDto>> GetAdminNotificationAsync(int adminId, bool? isRead);
    Task<int> GetUnreadCountForOpsAsync();
    Task<int> GetUnreadCountForAdminAsync(int adminId);
    Task MarkOpsAsReadAsync();
    Task MarkAdminAsReadAsync(int adminId);
}