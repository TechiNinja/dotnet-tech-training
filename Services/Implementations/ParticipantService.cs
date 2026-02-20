using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class ParticipantService: IParticipantService
    {
        private readonly IParticipantRepository _participantRepository;

        public ParticipantService(IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public Task<List<MyEventsDto>> GetMyEventsAsync(int userId)
        {
            return _participantRepository.GetMyEventsAsync(userId);
        }

        public Task<List<MyTeamDto>> GetMyTeamsAsync(int userId)
        {
            return _participantRepository.GetMyTeamsAsync(userId);
        }

        public Task<List<MyScheduleDto>> GetMySchedulesAsync(int userId)
        {
            return _participantRepository.GetMyScheduleAsync(userId);
        }
    }
}
