using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<Match?> GetByIdWithSetsAndResultAsync(int matchId);
        Task<IEnumerable<Match>> GetByCategoryAsync(int catId, string? status);
        Task<List<MatchSet>> GetSetsAsync(int matchId);
        Task UpdateEventStatusAsync(int eventCategoryId, EventStatus status);
        Task DeleteAllByCategoryAsync(int catId);
        Task<bool> AllMatchesCompletedAsync(int eventCategoryId);
        Task<Match?> GetByRoundAndBracketAsync(int catId, int roundNumber, int bracketPosition);
    }
}