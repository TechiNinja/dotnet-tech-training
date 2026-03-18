using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryService : IGenericService<EventCategoryResponseDto>
    {
        Task<FixtureResponseDto> GetMatchByIdAsync(int matchId);
    }
}