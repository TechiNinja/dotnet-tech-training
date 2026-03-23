using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;

namespace SportsManagementApp.Repositories.Implementations
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<Event?> GetByIdWithDetailsAsync(int eventId) =>
            await _context.Events
                .Include(e => e.Sport)
                .Include(e => e.Organizer)
                .Include(e => e.EventRequest)
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.Id == eventId);
    }
}