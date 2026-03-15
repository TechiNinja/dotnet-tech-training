using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task<List<Notification>> GetOpsAsync()
    {
        var predicate = NotificationPredicateBuilder.Ops();

        return await _context.Notifications
            .AsNoTracking()
            .Where(predicate)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAdminAsync(int adminId)
    {
        var predicate = NotificationPredicateBuilder.Admin(adminId);

        return await _context.Notifications
            .AsNoTracking()
            .Where(predicate)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}