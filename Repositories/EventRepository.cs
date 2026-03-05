using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;

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

        public async Task<IEnumerable<EventResponse>> GetProjectedListAsync(
            string? status, string? name, int? sportId)
        {
            var query = _context.Events.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<EventStatus>(status, true, out var parsedStatus))
                query = query.Where(new EventByStatusSpec(parsedStatus));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(new EventByNameContainsSpec(name));

            if (sportId.HasValue)
                query = query.Where(new EventBySportSpec(sportId.Value));

            return await query
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new EventResponse
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
                    Categories           = e.Categories.Select(c => new EventCategoryResponse
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
            await ExistsAsync(new EventByRequestIdSpec(requestId));
    }
}
