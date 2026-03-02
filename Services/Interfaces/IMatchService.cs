using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IMatchService
    {
        Task<MatchSetResponse>              UpdateSetAsync(int matchId, MatchSetRequest request);
        Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId);
        Task<MatchResultResponse>           SubmitResultAsync(int matchId);
        Task<MatchResultResponse>           GetResultAsync(int matchId);
    }
}