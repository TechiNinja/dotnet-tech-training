using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;

namespace SportsManagementApp.Repositories.Implementations
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<Match?> GetByIdWithSetsAndResultAsync(int matchId) =>
            await _context.Matches
                .Include(m => m.MatchSets)
                .Include(m => m.Result)
                .FirstOrDefaultAsync(new MatchByIdSpec(matchId).ToExpression());

        public async Task<IEnumerable<Match>> GetByCategoryAsync(int catId, string? status)
        {
            ISpecification<Match> spec = new MatchByCategorySpec(catId);
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                spec = spec.And(new MatchByStatusSpec(parsedStatus));

            return await _context.Matches
                .Include(m => m.MatchSets)
                .Include(m => m.Result)
                .AsNoTracking()
                .Where(spec.ToExpression())
                .OrderBy(m => m.RoundNumber)
                .ThenBy(m => m.MatchNumber)
                .ToListAsync();
        }

        public async Task<List<MatchSet>> GetSetsAsync(int matchId) =>
            await _context.MatchSets
                .AsNoTracking()
                .Where(s => s.MatchId == matchId)
                .OrderBy(s => s.SetNumber)
                .ToListAsync();

        public async Task UpdateEventStatusAsync(int eventCategoryId, EventStatus status) =>
            await _context.Events
                .Where(e => e.Categories.Any(c => c.Id == eventCategoryId))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.Status, status)
                    .SetProperty(e => e.UpdatedAt, DateTime.UtcNow));

        public async Task DeleteAllByCategoryAsync(int catId) =>
            await _context.Matches
                .Where(new MatchByCategorySpec(catId).ToExpression())
                .ExecuteDeleteAsync();

        public async Task<bool> AllMatchesCompletedAsync(int eventCategoryId) =>
            await _context.Matches
                .Where(new MatchByCategorySpec(eventCategoryId).ToExpression())
                .AllAsync(m => m.Status == MatchStatus.Completed ||
                               (m.Status == MatchStatus.Upcoming && m.SideAId == null && m.SideBId == null));

        public async Task<Match?> GetByRoundAndBracketAsync(int catId, int round, int bracketPos) =>
            await _context.Matches
                .FirstOrDefaultAsync(new MatchByRoundAndBracketSpec(catId, round, bracketPos).ToExpression());
    }
}