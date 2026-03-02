using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse>             GetByIdAsync(int catId);
        Task<IEnumerable<FixtureResponse>> GenerateFixturesAsync(int catId);
        Task<IEnumerable<FixtureResponse>> GetFixturesAsync(int catId, string? status);
        Task                               DeleteFixturesAsync(int catId);
        Task<FixtureResponse>              GetMatchByIdAsync(int matchId);
        Task<IEnumerable<FixtureResponse>> BulkScheduleAsync(int catId, BulkScheduleRequest request);
    }
}