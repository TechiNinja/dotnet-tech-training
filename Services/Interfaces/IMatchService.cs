using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IMatchService
    {
        Task<FixtureResponseDto> RescheduleAsync(int matchId, RescheduleRequestDto request);
        Task<SetUpdateResponseDto> UpdateSetAsync(int matchId, MatchSetRequestDto request);
        Task<SetUpdateResponseDto> UpdateSetByIdAsync(int matchId, int setId, MatchSetRequestDto request);
        Task<IEnumerable<MatchSetResponseDto>> GetSetsAsync(int matchId);
    }
}