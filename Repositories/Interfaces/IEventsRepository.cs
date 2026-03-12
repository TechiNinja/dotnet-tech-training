using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventsRepository
    {
        Task<List<MyEventsDto>> GetUserEventsAsync(int userId);
    }
}           