using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
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

        public async Task<IEnumerable<EventResponseDto>> GetProjectedListAsync(EventFilterDto filter)
        {
            var predicate = EventPredicateBuilder.Build(filter);

            return await _context.Events
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EventResponseDto
                {
                    Id                   = e.Id,
                    Name                 = e.Name,
                    SportName            = e.Sport != null ? e.Sport.Name : string.Empty,
                    StartDate            = e.StartDate,
                    EndDate              = e.EndDate,
                    EventVenue           = e.EventVenue,
                    RegistrationDeadline = e.RegistrationDeadline,
                    MaxParticipantsCount = e.MaxParticipantsCount,
                    TournamentType       = e.TournamentType.ToString(),
                    OrganizerName        = e.Organizer != null ? e.Organizer.FullName : null,
                    Status               = e.Status.ToString(),
                    Description          = e.Description,
                    CreatedAt            = e.CreatedAt,
                    UpdatedAt            = e.UpdatedAt,
                    Categories           = e.Categories.Select(c => new EventCategoryResponseDto
                    {
                        Id        = c.Id,
                        Gender    = c.Gender.ToString(),
                        Format    = c.Format.ToString(),
                        Status    = c.Status.ToString(),
                        CreatedAt = c.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsByRequestIdAsync(int requestId) =>
            await ExistsAsync(new EventByRequestIdSpec(requestId).ToExpression());
    }
}