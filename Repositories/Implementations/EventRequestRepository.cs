using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
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

        public async Task<EventRequest?> GetEventRequestById(int id) =>
            await _context.EventRequests
                .Include(e => e.Sport)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<EventRequest>> Search(int? id, RequestStatus? status)
        {
            var query = _context.EventRequests
                .Include(e => e.Sport)
                .AsQueryable();

            if (id.HasValue)
                query = query.Where(e => e.Id == id.Value);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query.ToListAsync();
        }
    }
}