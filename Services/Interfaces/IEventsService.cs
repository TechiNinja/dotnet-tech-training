using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventsService
    {
        Task<List<MyEventsDto>> GetUserEventsAsync(int userId);
    }
}
