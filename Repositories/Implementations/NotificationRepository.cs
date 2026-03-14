using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Notification>> GetOpsAsync()
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.Audience == NotificationAudience.Ops)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAdminAsync(int adminId)
    {
        return await _context.Notifications
            .AsNoTracking()
            .Where(n => n.Audience == NotificationAudience.Admin && n.UserId == adminId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}