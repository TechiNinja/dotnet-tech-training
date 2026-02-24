using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class SchedulesService: ISchedulesService
    {
        private readonly ISchedulesRepository _schedulesRepository;

        public SchedulesService(ISchedulesRepository schedulesRepository)
        {
            _schedulesRepository = schedulesRepository;
        }

        public async Task<List<MyScheduleDto>> GetUserSchedulesAsync(int userId)
        {
            return await _schedulesRepository.GetUserScheduleAsync(userId);
        }
    }
}
