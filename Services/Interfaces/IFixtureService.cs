using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IFixtureService
    {
        Task<IEnumerable<FixtureResponseDto>> GenerateFixturesAsync(int catId);
        Task<IEnumerable<FixtureResponseDto>> GetFixturesAsync(int catId, string? status);
        Task DeleteFixturesAsync(int catId);
    }
}