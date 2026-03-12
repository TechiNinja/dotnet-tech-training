using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IFixtureService
    {
        Task<IEnumerable<FixtureResponse>> GenerateFixturesAsync(int catId);
        Task<IEnumerable<FixtureResponse>> GetFixturesAsync(int catId, string? status);
        Task<IEnumerable<FixtureResponse>> BulkScheduleAsync(int catId, List<MatchScheduleItem> schedules);
        Task DeleteFixturesAsync(int catId);
    }
}
