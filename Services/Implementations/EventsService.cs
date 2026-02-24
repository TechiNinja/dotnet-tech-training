using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class EventsService: IEventsService
    {
        private readonly IEventsRepository _eventsRepository;

        public EventsService(IEventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<List<MyEventsDto>> GetUserEventsAsync(int userId)
        {
            return await _eventsRepository.GetUserEventsAsync(userId);
        }
    }
}
