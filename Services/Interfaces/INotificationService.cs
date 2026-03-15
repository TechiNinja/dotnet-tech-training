using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateAsync(CreateNotificationDto dto);
    Task<List<Notification>> GetOpsAsync();
    Task<List<Notification>> GetAdminAsync(int adminId);
}