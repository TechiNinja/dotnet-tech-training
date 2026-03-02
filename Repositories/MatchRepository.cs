using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(AppDbContext context) : base(context) { }

        public async Task<Match?> GetByIdWithSetsAndResultAsync(int matchId) =>
            await _context.Matches
                .Include(m => m.MatchSets)
                .Include(m => m.Result)
                .FirstOrDefaultAsync(m => m.Id == matchId);

        public async Task<IEnumerable<Match>> GetByCategoryAsync(int catId, string? status)
        {
            var query = _context.Matches
                .Include(m => m.MatchSets)
                .Include(m => m.Result)
                .Where(m => m.EventCategoryId == catId)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                query = query.Where(m => m.Status == parsedStatus);

            return await query
                .OrderBy(m => m.RoundNumber)
                .ThenBy(m => m.MatchNumber)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(int catId, DateTime matchDateTime, int? excludeMatchId = null)
        {
            var buffer = TimeSpan.FromMinutes(60);
            return await _context.Matches
                .Where(m => m.EventCategoryId == catId
                    && (excludeMatchId == null || m.Id != excludeMatchId)
                    && m.MatchDateTime >= matchDateTime.Subtract(buffer)
                    && m.MatchDateTime <= matchDateTime.Add(buffer))
                .AnyAsync();
        }

        public async Task AddResultAsync(Result result)
        {
            await _context.Results.AddAsync(result);
        }

        public async Task DeleteAllByCategoryAsync(int catId)
        {
            var matches = await _context.Matches
                .Where(m => m.EventCategoryId == catId)
                .ToListAsync();
            _context.Matches.RemoveRange(matches);
        }

        public async Task<MatchSet?> GetSetBySetNumberAsync(int matchId, int setNumber) =>
            await _context.MatchSets
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.SetNumber == setNumber);

        public void UpdateSet(MatchSet set)
        {
            _context.MatchSets.Update(set);
        }
    }
}