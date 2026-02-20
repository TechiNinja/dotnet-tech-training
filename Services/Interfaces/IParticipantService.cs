using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IParticipantService
    {
        Task<List<MyEventsDto>> GetMyEventsAsync(int userId);
        Task<List<MyTeamDto>> GetMyTeamsAsync(int userId);
        Task<List<MyScheduleDto>> GetMySchedulesAsync(int userId);
    }
}
