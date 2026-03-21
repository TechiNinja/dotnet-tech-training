using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class EventCategoryRepository : GenericRepository<EventCategory>, IEventCategoryRepository
    {
        public EventCategoryRepository(AppDbContext context) : base(context) { }

        public async Task<EventCategory?> GetByIdWithDetailsAsync(int catId) =>
            await _context.EventCategories
                .Include(c => c.Event)
                .Include(c => c.Teams)
                .Include(c => c.EventRegistrations)
                    .ThenInclude(r => r.User)
                .Include(c => c.Matches)
                .FirstOrDefaultAsync(c => c.Id == catId);
    }
}