using AutoMapper;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IGenericRepository<Result> _resultRepo;
        private readonly IGenericRepository<MatchSet> _setRepo;
        private readonly IMapper _mapper;

        public MatchService(
            IMatchRepository matchRepo,
            IGenericRepository<Result> resultRepo,
            IGenericRepository<MatchSet> setRepo,
            IMapper mapper)
        {
            _matchRepo = matchRepo;
            _resultRepo = resultRepo;
            _setRepo = setRepo;
            _mapper = mapper;
        }

        public async Task<SetUpdateResponseDto> UpdateSetAsync(int matchId, MatchSetRequestDto request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.MatchAlreadyCompleted, matchId));

            var liveSet = match.MatchSets.FirstOrDefault(s => s.Status == SetStatus.Live);
            var updatedSet = liveSet != null
                ? await ApplyScoreToLiveSetAsync(liveSet, request)
                : await CreateNextSetAsync(match, request);

            match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            MatchResultResponseDto? result = null;
            if (AllSetsCompleted(match))
                result = await SubmitResultAsync(matchId);

            return new SetUpdateResponseDto
            {
                Set = _mapper.Map<MatchSetResponseDto>(updatedSet),
                Result = result
            };
        }

        public async Task<SetUpdateResponseDto> UpdateSetByIdAsync(int matchId, int setId, MatchSetRequestDto request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            var set = match.MatchSets.FirstOrDefault(s => s.Id == setId)
                ?? throw new NotFoundException(string.Format(StringConstant.SetNotFound, setId, matchId));

            _mapper.Map(request, set);
            set.UpdatedAt = DateTime.UtcNow;
            await _setRepo.UpdateAsync(set);

            return new SetUpdateResponseDto { Set = _mapper.Map<MatchSetResponseDto>(set) };
        }

        public async Task<IEnumerable<MatchSetResponseDto>> GetSetsAsync(int matchId)
        {
            if (!await _matchRepo.ExistsAsync(m => m.Id == matchId))
                throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            var sets = await _matchRepo.GetSetsAsync(matchId);
            return _mapper.Map<IEnumerable<MatchSetResponseDto>>(sets);
        }

        private static bool AllSetsCompleted(Match match) =>
            match.MatchSets.Any() &&
            match.MatchSets.All(s => s.Status != SetStatus.Live) &&
            (match.TotalSets == 0 || match.MatchSets.Count(s => s.Status == SetStatus.Completed) >= match.TotalSets);

        private static bool IsFinal(Match match, int lastRound) => match.RoundNumber == lastRound && match.BracketPosition == 0;
        private static bool IsByeMatch(Match match, int lastRound) => match.RoundNumber == lastRound && match.BracketPosition == 1;
        private static bool IsOddBracket(List<Match> all, int last) => all.Any(m => m.RoundNumber == last && m.BracketPosition == 1);

        private async Task<MatchSet> ApplyScoreToLiveSetAsync(MatchSet liveSet, MatchSetRequestDto request)
        {
            _mapper.Map(request, liveSet);
            liveSet.UpdatedAt = DateTime.UtcNow;
            if (request.IsCompleted) liveSet.Status = SetStatus.Completed;
            await _setRepo.UpdateAsync(liveSet);
            return liveSet;
        }

        private async Task<MatchSet> CreateNextSetAsync(Match match, MatchSetRequestDto request)
        {
            ValidateNewSetAllowed(match);

            var allMatches = (await _matchRepo.GetByCategoryAsync(match.EventCategoryId, null)).ToList();

            if (allMatches.Any(m => m.RoundNumber == match.RoundNumber - 1 && m.Status != MatchStatus.Completed))
                throw new UnprocessableEntityException(StringConstant.PreviousRoundNotCompleted);

            var newSet = _mapper.Map<MatchSet>(request);
            newSet.MatchId = match.Id;
            newSet.SetNumber = match.MatchSets.Any() ? match.MatchSets.Max(s => s.SetNumber) + 1 : 1;
            newSet.Status = request.IsCompleted ? SetStatus.Completed : SetStatus.Live;
            newSet.CreatedAt = DateTime.UtcNow;

            if (match.Status == MatchStatus.Upcoming)
            {
                match.Status = MatchStatus.Live;
                match.UpdatedAt = DateTime.UtcNow;
                await _matchRepo.UpdateAsync(match);
                await _matchRepo.UpdateEventStatusAsync(match.EventCategoryId, EventStatus.Live);
            }

            match.MatchSets.Add(newSet);
            await _matchRepo.SaveChangesAsync();
            return newSet;
        }

        private static void ValidateNewSetAllowed(Match match)
        {
            if (match.TotalSets == 0)
            {
                if (match.MatchSets.Any())
                    throw new ConflictException(
                        string.Format(StringConstant.MatchSetAlreadyExists, 1, match.Id));
                return;
            }

            if (match.MatchSets.Count >= match.TotalSets)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.MaxSetsReached, match.TotalSets));

            int next = match.MatchSets.Count + 1;
            if (next > 1)
            {
                var prev = match.MatchSets.FirstOrDefault(s => s.SetNumber == next - 1);
                if (prev?.Status != SetStatus.Completed)
                    throw new UnprocessableEntityException(
                        string.Format(StringConstant.PreviousSetNotCompleted, next - 1, next));
            }
        }

        private async Task<MatchResultResponseDto> SubmitResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new ConflictException(string.Format(StringConstant.MatchAlreadyCompleted, matchId));

            if (!match.MatchSets.Any())
                throw new UnprocessableEntityException(StringConstant.NoScoreSubmitted);

            if (match.MatchSets.Any(s => s.Status == SetStatus.Live) ||
               (match.TotalSets > 0 && match.MatchSets.Count(s => s.Status == SetStatus.Completed) < match.TotalSets))
                throw new UnprocessableEntityException(StringConstant.AllSetsNotCompleted);

            int sideAWins = match.MatchSets.Count(s => s.ScoreA > s.ScoreB);
            int sideBWins = match.MatchSets.Count(s => s.ScoreB > s.ScoreA);

            if (sideAWins == sideBWins)
                throw new UnprocessableEntityException(StringConstant.DrawNotAllowed);

            bool aWins = sideAWins > sideBWins;
            int? winnerId = aWins ? match.SideAId : match.SideBId;
            int? loserId = aWins ? match.SideBId : match.SideAId;

            await _resultRepo.AddAsync(new Result
            {
                MatchId = matchId,
                WinnerId = winnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            match.Status = MatchStatus.Completed;
            match.UpdatedAt = DateTime.UtcNow;
            await _matchRepo.UpdateAsync(match);

            var allMatches = (await _matchRepo.GetByCategoryAsync(match.EventCategoryId, null)).ToList();
            int lastRound = allMatches.Max(m => m.RoundNumber);
            bool oddBracket = IsOddBracket(allMatches, lastRound);

            if (IsFinal(match, lastRound)) { }
            else if (IsByeMatch(match, lastRound)) await AdvanceByeMatchWinnerAsync(match, winnerId, allMatches, lastRound);
            else if (oddBracket && match.RoundNumber == lastRound - 1 && match.BracketPosition == 0) await AdvanceSemiFinalAsync(match, winnerId, loserId, allMatches);
            else await AdvanceRegularWinnerAsync(match, winnerId, allMatches, lastRound, oddBracket);

            if (await _matchRepo.AllMatchesCompletedAsync(match.EventCategoryId))
                await _matchRepo.UpdateEventStatusAsync(match.EventCategoryId, EventStatus.Completed);

            var saved = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId);
            return _mapper.Map<MatchResultResponseDto>(saved!.Result);
        }

        private async Task AdvanceRegularWinnerAsync(Match completedMatch, int? winnerId, List<Match> allMatches, int lastRound, bool oddBracket)
        {
            var currentRoundMatches = allMatches
                .Where(m => m.RoundNumber == completedMatch.RoundNumber)
                .OrderBy(m => m.BracketPosition)
                .ToList();

            int positionInRound = currentRoundMatches.FindIndex(m => m.Id == completedMatch.Id);
            if (positionInRound < 0) return;

            var nextMatch = allMatches
                .Where(m => m.RoundNumber == completedMatch.RoundNumber + 1 && !IsByeMatch(m, lastRound))
                .OrderBy(m => m.BracketPosition)
                .ElementAtOrDefault(positionInRound / 2);

            if (nextMatch is null) return;

            if (positionInRound % 2 == 0) nextMatch.SideAId = winnerId;
            else nextMatch.SideBId = winnerId;

            nextMatch.UpdatedAt = DateTime.UtcNow;
            await _matchRepo.UpdateAsync(nextMatch);
        }

        private async Task AdvanceSemiFinalAsync(Match semiFinal, int? winnerId, int? loserId, List<Match> allMatches)
        {
            int nextRound = semiFinal.RoundNumber + 1;
            var finalMatch = allMatches.FirstOrDefault(m => m.RoundNumber == nextRound && m.BracketPosition == 0);
            var byeMatch = allMatches.FirstOrDefault(m => m.RoundNumber == nextRound && m.BracketPosition == 1);

            if (finalMatch is not null) { finalMatch.SideAId = winnerId; finalMatch.UpdatedAt = DateTime.UtcNow; await _matchRepo.UpdateAsync(finalMatch); }
            if (byeMatch is not null) { byeMatch.SideBId = loserId; byeMatch.UpdatedAt = DateTime.UtcNow; await _matchRepo.UpdateAsync(byeMatch); }
        }

        private async Task AdvanceByeMatchWinnerAsync(Match byeMatch, int? winnerId, List<Match> allMatches, int lastRound)
        {
            var finalMatch = allMatches.FirstOrDefault(m => m.RoundNumber == byeMatch.RoundNumber && IsFinal(m, lastRound));
            if (finalMatch is null) return;

            finalMatch.SideBId = winnerId;
            finalMatch.UpdatedAt = DateTime.UtcNow;
            await _matchRepo.UpdateAsync(finalMatch);
        }
    }
}