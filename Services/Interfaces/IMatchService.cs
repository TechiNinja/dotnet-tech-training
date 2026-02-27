using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IMatchService
    {
        Task<MatchSetResponse>              StartSetAsync(int matchId);
        Task<MatchSetResponse>              UpdateSetScoreAsync(int matchId, int setId, MatchSetRequest request);
        Task<MatchSetResponse>              CompleteSetAsync(int matchId, int setId);
        Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId);
        Task<MatchResultResponse>           SubmitResultAsync(int matchId);
        Task<MatchResultResponse>           GetResultAsync(int matchId);
    }
}