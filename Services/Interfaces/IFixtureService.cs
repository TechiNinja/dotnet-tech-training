using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IFixtureService
    {
        Task<IEnumerable<FixtureResponseDto>> GenerateFixturesAsync(int catId);
        Task<IEnumerable<FixtureResponseDto>> GetFixturesAsync(int catId, string? status);
        Task<FixtureResponseDto> RescheduleAsync(int catId, RescheduleRequestDto request);
        Task DeleteFixturesAsync(int catId);
    }
}
