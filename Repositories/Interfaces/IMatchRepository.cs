using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<Match?>            GetByIdWithSetsAndResultAsync(int matchId);
        Task<IEnumerable<Match>> GetByCategoryAsync(int catId, string? status);
        Task                    AddResultAsync(Result result);
        Task<bool>              HasOverlapAsync(int catId, DateTime matchDateTime, int? excludeMatchId = null);
        Task                    DeleteAllByCategoryAsync(int catId);
        Task<MatchSet?>         GetSetByIdAsync(int matchId, int setId);
        void                    UpdateSet(MatchSet set);
    }
}