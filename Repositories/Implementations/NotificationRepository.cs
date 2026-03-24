using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Notification>> GetOpsNotificationAsync(bool? isRead)
    {
        var notifications = await GetAllWithIncludesAsync(
            NotificationPredicateBuilder.Ops(isRead)
        );

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public async Task<List<Notification>> GetAdminNotificationAsync(int adminId, bool? isRead)
    {
        var notifications = await GetAllWithIncludesAsync(
            NotificationPredicateBuilder.Admin(adminId, isRead)
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