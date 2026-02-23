using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class EventsRepository: IEventsRepository
    {
        private readonly AppDbContext _context;

        public EventsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MyEventsDto>> GetUserEventsAsync(int userId)
        {
            return await _context.ParticipantRegistrations
                .Where(registration => registration.UserId == userId
                      && registration.EventCategory != null
                      && registration.EventCategory.Event != null)
                .Include(registration => registration.EventCategory)
                .Select(registration => new MyEventsDto
                {
                    EventId = registration.EventCategory!.EventId,
                    EventName = registration.EventCategory!.Event!.Name,
                    StartDate = registration.EventCategory.Event.StartDate,
                    EndDate = registration.EventCategory.Event.EndDate
                })
                .ToListAsync();
        }
    }
}
