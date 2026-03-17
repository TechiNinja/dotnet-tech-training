using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateAsync(CreateNotificationDto dto);
    Task<List<NotificationResponseDto>> GetOpsAsync();
    Task<List<NotificationResponseDto>> GetAdminAsync(int adminId);
    Task<int> GetUnreadCountForOpsAsync();
    Task<int> GetUnreadCountForAdminAsync(int adminId);
    Task MarkOpsAsReadAsync();
    Task MarkAdminAsReadAsync(int adminId);
}