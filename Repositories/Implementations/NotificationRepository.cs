using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<NotificationResponseDto>> GetOpsAsync()
    {
        var notifications = await GetAllAsync(
            NotificationPredicateBuilder.Ops(),
            NotificationProjectionBuilder.ToDto()
        );

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public async Task<List<NotificationResponseDto>> GetAdminAsync(int adminId)
    {
        var notifications = await GetAllAsync(
            NotificationPredicateBuilder.Admin(adminId),
            NotificationProjectionBuilder.ToDto()
        );

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public async Task<int> GetUnreadCountForOpsAsync()
    {
        return await CountAsync(NotificationPredicateBuilder.UnreadOps());
    }

    public async Task<int> GetUnreadCountForAdminAsync(int adminId)
    {
        return await CountAsync(NotificationPredicateBuilder.UnreadAdmin(adminId));
    }

    public async Task MarkOpsAsReadAsync()
    {
        var notifications = await _context.Notifications
            .Where(n => n.Audience == NotificationAudience.Ops && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
    }

    public async Task MarkAdminAsReadAsync(int adminId)
    {
        var notifications = await _context.Notifications
            .Where(n =>
                n.Audience == NotificationAudience.Admin &&
                n.UserId == adminId &&
                !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
    }
}