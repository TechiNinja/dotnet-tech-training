using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class EventRequestRepository : GenericRepository<EventRequest>, IEventRequestRepository
{
    public EventRequestRepository(AppDbContext context) : base(context) { }

    public async Task<EventRequest?> GetEventRequestByIdAsync(int id)
    {
        return await GetByIdWithIncludesAsync(e => e.Id == id, e => e.Sport, e => e.OperationsReviewer, e => e.Admin);
    }

    public async Task<List<EventRequest>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter)
    {
        var predicate = EventRequestPredicateBuilder.Build(filter);

        return await GetAllWithIncludesAsync(predicate, e => e.Sport, e => e.Admin, e => e.OperationsReviewer);
    }
}