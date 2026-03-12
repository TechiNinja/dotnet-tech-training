using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ISchedulesRepository
    {
        Task<List<MyScheduleDto>> GetUserScheduleAsync(int userId);
    }
}
