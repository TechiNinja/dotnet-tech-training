using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> GetByIdAsync(int catId);
        Task<FixtureResponse>  GetMatchByIdAsync(int matchId);
    }
}
