using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;
using SportsManagementApp.StringConstants;

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
            ISpecification<Match> spec = new MatchByCategorySpec(catId);
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<MatchStatus>(status, true, out var parsedStatus))
                spec = spec.And(new MatchByStatusSpec(parsedStatus));

            return await _context.Matches
                .Include(m => m.MatchSets)
                .Include(m => m.Result)
                .AsNoTracking()
                .Where(spec)
                .OrderBy(m => m.RoundNumber)
                .ThenBy(m => m.MatchNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchSetResponse>> GetSetsProjectedAsync(int matchId) =>
            await _context.MatchSets
                .AsNoTracking()
                .Where(s => s.MatchId == matchId)
                .OrderBy(s => s.SetNumber)
                .Select(s => new MatchSetResponse
                {
                    Id        = s.Id,
                    MatchId   = s.MatchId,
                    SetNumber = s.SetNumber,
                    ScoreA    = s.ScoreA,
                    ScoreB    = s.ScoreB,
                    Status    = s.Status.ToString(),
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

        public async Task<bool> HasOverlapAsync(int catId, DateTime matchDateTime, int? excludeMatchId = null)
        {
            var buffer = TimeSpan.FromMinutes(AppConstants.ScheduleOverlapMinutes);
            ISpecification<Match> spec = new MatchByCategorySpec(catId)
                .And(new MatchWithinTimeWindowSpec(matchDateTime, buffer));
            if (excludeMatchId.HasValue)
                spec = spec.And(new MatchExcludeIdSpec(excludeMatchId.Value));
            return await ExistsAsync(spec);
        }

        public async Task AddResultAsync(Result result) =>
            await _context.Results.AddAsync(result);

        public async Task DeleteAllByCategoryAsync(int catId) =>
            await _context.Matches
                .Where(new MatchByCategorySpec(catId))
                .ExecuteDeleteAsync();

        public async Task<MatchSet?> GetSetBySetNumberAsync(int matchId, int setNumber) =>
            await _context.MatchSets
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.SetNumber == setNumber);

        public void UpdateSet(MatchSet set) => _context.MatchSets.Update(set);

        public async Task<Match?> GetByRoundAndBracketAsync(int catId, int round, int bracketPos) =>
            await _context.Matches
                .FirstOrDefaultAsync(m =>
                    m.EventCategoryId == catId     &&
                    m.RoundNumber     == round     &&
                    m.BracketPosition == bracketPos);
    }
}