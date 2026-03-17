using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IMatchService
    {
        Task<SetUpdateResponseDto> UpdateSetAsync(int matchId, MatchSetRequestDto request);
        Task<IEnumerable<MatchSetResponseDto>> GetSetsAsync(int matchId);
        Task<SetUpdateResponseDto> UpdateSetByIdAsync(int matchId, int setId, MatchSetRequestDto request);
    }
}