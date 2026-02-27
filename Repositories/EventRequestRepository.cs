using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class EventRequestRepository : GenericRepository<EventRequest>, IEventRequestRepository
    {
        public EventRequestRepository(AppDbContext context) : base(context) { }

        public async Task<EventRequest?> GetByIdWithDetailsAsync(int id) =>
            await _context.EventRequests
                .Include(r => r.Sport)
                .Include(r => r.Admin)
                .Include(r => r.OperationsReviewer)
                .FirstOrDefaultAsync(r => r.Id == id);
    }
}