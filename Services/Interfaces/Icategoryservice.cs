using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<EventCategoryResponseDto>  GetByIdAsync(int catId);
        Task<FixtureResponseDto> GetMatchByIdAsync(int matchId);
    }
}