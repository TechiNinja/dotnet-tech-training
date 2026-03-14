using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
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

        public async Task<IEnumerable<EventRequest>> Search(int? id, RequestStatus? status) =>
            await _context.EventRequests
                .Include(e => e.Sport)
                .Where(e => (!id.HasValue || e.Id == id.Value) &&
                            (!status.HasValue || e.Status == status.Value))
                .ToListAsync();

        public async Task<EventRequestResponseDto?> GetEventRequestDtoByIdAsync(int id)
        {
            return await _context.EventRequests
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(EventRequestProjectionBuilder.Build())
                .FirstOrDefaultAsync();
        }

        public async Task<EventRequest?> GetEventRequestByIdAsync(int id)
        {
            return await _context.EventRequests
                .Include(e => e.Sport)
                .Include(e => e.OperationsReviewer)
                .Include(e => e.Admin)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<EventRequestResponseDto>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter)
        {
            var predicate = EventRequestPredicateBuilder.Build(filter);
            return await _context.EventRequests
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(e => e.CreatedDate)
                .Select(EventRequestProjectionBuilder.Build())
                .ToListAsync();
        }
    }
}