using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IMatchService
    {
        Task<SetUpdateResponse> UpdateSetAsync(int matchId, MatchSetRequest request);
        Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId);
        Task<SetUpdateResponse> UpdateSetByIdAsync(int matchId, int setId, MatchSetRequest request);
    }
}