using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class EventRequestRepository : GenericRepository<EventRequest>, IEventRequestRepository
{
    public EventRequestRepository(AppDbContext context) : base(context) { }

    public async Task<EventRequest?> GetEventRequestByIdAsync(int id)
{
    return await GetByIdWithIncludesAsync(
        e => e.Id == id,
        e => e.Sport,
        e => e.OperationsReviewer,
        e => e.Admin
    );
}

    public async Task<EventRequestResponseDto?> GetEventRequestDtoByIdAsync(int id)
    {
        return await _context.EventRequests
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(EventRequestProjectionBuilder.Build())
            .FirstOrDefaultAsync();
    }

    public async Task<List<EventRequestResponseDto>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter)
{
    var predicate = EventRequestPredicateBuilder.Build(filter);

    return await GetAllAsync(
        predicate,
        EventRequestProjectionBuilder.Build()
    );
}
}