using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IParticipantRepository
    {
        Task<List<MyEventsDto>> GetMyEventsAsync(int userId);
        Task<List<MyScheduleDto>> GetMyScheduleAsync(int userId);
        Task<List<MyTeamDto>> GetMyTeamsAsync(int userId);
    }
}
