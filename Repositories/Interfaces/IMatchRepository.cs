using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<Match?>                        GetByIdWithSetsAndResultAsync(int matchId);
        Task<IEnumerable<Match>>            GetByCategoryAsync(int catId, string? status);
        Task<IEnumerable<MatchSetResponse>> GetSetsProjectedAsync(int matchId);
        Task<bool>                          HasOverlapAsync(int catId, DateTime matchDateTime, int? excludeMatchId = null);
        Task                                AddResultAsync(Result result);
        Task                                DeleteAllByCategoryAsync(int catId);
        Task<MatchSet?>                     GetSetBySetNumberAsync(int matchId, int setNumber);
        void                                UpdateSet(MatchSet set);
        Task<Match?>                        GetByRoundAndBracketAsync(int catId, int roundNumber, int bracketPosition); // ← added
    }
}