using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ISchedulesService
    {
        Task<List<MyScheduleDto>> GetUserSchedulesAsync(int userId);
    }
}
