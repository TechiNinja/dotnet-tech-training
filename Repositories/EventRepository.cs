using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<Event?> GetByIdWithDetailsAsync(int id) =>
            await _context.Events
                .Include(e => e.Sport)
                .Include(e => e.Organizer)
                .Include(e => e.EventRequest)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Event>> GetAllWithCategoriesAsync(string? status, string? name, int? sportId)
        {
            var query = _context.Events
                .Include(e => e.Sport)
                .Include(e => e.Organizer)
                .Include(e => e.Categories)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EventStatus>(status, true, out var parsedStatus))
                query = query.Where(e => e.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.Name.Contains(name));

            if (sportId.HasValue)
                query = query.Where(e => e.SportId == sportId.Value);

            return await query.OrderByDescending(e => e.CreatedAt).ToListAsync();
        }

        public async Task<bool> ExistsByRequestIdAsync(int requestId) =>
            await _context.Events.AnyAsync(e => e.EventRequestId == requestId);
    }
}