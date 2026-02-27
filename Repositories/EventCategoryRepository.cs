using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class EventCategoryRepository : GenericRepository<EventCategory>, IEventCategoryRepository
    {
        public EventCategoryRepository(AppDbContext context) : base(context) { }

        public async Task<EventCategory?> GetByIdWithDetailsAsync(int catId) =>
            await _context.EventCategories
                .Include(c => c.Event)
                .Include(c => c.Teams)
                    .ThenInclude(t => t.Members)
                        .ThenInclude(m => m.User)
                .Include(c => c.EventRegistrations)
                    .ThenInclude(r => r.User)
                .Include(c => c.Matches)
                    .ThenInclude(m => m.MatchSets)
                .Include(c => c.Matches)
                    .ThenInclude(m => m.Result)
                .FirstOrDefaultAsync(c => c.Id == catId);

    }
}