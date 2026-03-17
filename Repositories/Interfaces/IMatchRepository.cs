using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<Match?> GetByIdWithSetsAndResultAsync(int matchId);
        Task<IEnumerable<Match>> GetByCategoryAsync(int catId, string? status);
        Task<IEnumerable<MatchSetResponseDto>> GetSetsProjectedAsync(int matchId);
        Task AddResultAsync(Result result);
        Task UpdateSetAsync(MatchSet set);
        Task UpdateEventStatusAsync(int eventCategoryId, EventStatus status);
        Task DeleteAllByCategoryAsync(int catId);
        Task<bool> AllMatchesCompletedAsync(int eventCategoryId);
        Task<Match?> GetByRoundAndBracketAsync(int catId, int roundNumber, int bracketPosition);
    }
}