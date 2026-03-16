using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IMapper _mapper;

        public MatchService(IMatchRepository matchRepo, IMapper mapper)
        {
            _matchRepo = matchRepo;
            _mapper    = mapper;
        }

        public async Task<SetUpdateResponse> UpdateSetAsync(int matchId, MatchSetRequest request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            var liveSet = match.MatchSets.FirstOrDefault(s => s.Status == SetStatus.Live);

            var updatedSet = liveSet != null
                ? await ApplyScoreToLiveSetAsync(liveSet, request)
                : await CreateNextSetAsync(match, request);

            match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            MatchResultResponse? result = null;
            if (AllSetsCompleted(match))
                result = await SubmitResultAsync(matchId);

            return new SetUpdateResponse
            {
                Set    = _mapper.Map<MatchSetResponse>(updatedSet),
                Result = result
            };
        }

        public async Task<SetUpdateResponse> UpdateSetByIdAsync(int matchId, int setId, MatchSetRequest request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            var set = match.MatchSets.FirstOrDefault(s => s.Id == setId)
                ?? throw new NotFoundException($"Set {setId} not found");

            _mapper.Map(request, set);
            set.UpdatedAt = DateTime.UtcNow;

            _matchRepo.UpdateSet(set);
            await _matchRepo.SaveChangesAsync();

            return new SetUpdateResponse { Set = _mapper.Map<MatchSetResponse>(set) };
        }

        public async Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId)
        {
            if (!await _matchRepo.ExistsAsync(m => m.Id == matchId))
                throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            return await _matchRepo.GetSetsProjectedAsync(matchId);
        }

        private static bool AllSetsCompleted(Match match)
        {
            if (!match.MatchSets.Any()) return false;
            if (match.MatchSets.Any(s => s.Status == SetStatus.Live)) return false;
            if (match.TotalSets > 0 &&
                match.MatchSets.Count(s => s.Status == SetStatus.Completed) < match.TotalSets)
                return false;
            return true;
        }

        private async Task<MatchSet> ApplyScoreToLiveSetAsync(MatchSet liveSet, MatchSetRequest request)
        {
            _mapper.Map(request, liveSet);
            liveSet.UpdatedAt = DateTime.UtcNow;
            if (request.IsCompleted) liveSet.Status = SetStatus.Completed;
            _matchRepo.UpdateSet(liveSet);
            await _matchRepo.SaveChangesAsync();
            return liveSet;
        }

        private async Task<MatchSet> CreateNextSetAsync(Match match, MatchSetRequest request)
        {
            ValidateNewSetAllowed(match);

            var allMatches = (await _matchRepo.GetByCategoryAsync(match.EventCategoryId, null)).ToList();

            if (allMatches.Any(m => m.RoundNumber == match.RoundNumber - 1 &&
                                    m.Status != MatchStatus.Completed))
                throw new UnprocessableEntityException(
                    "Previous round matches must be completed before starting this match.");

            int setNumber = match.MatchSets.Any()
                ? match.MatchSets.Max(s => s.SetNumber) + 1
                : 1;

            var newSet = _mapper.Map<MatchSet>(request);
            newSet.MatchId   = match.Id;
            newSet.SetNumber = setNumber;
            newSet.Status    = request.IsCompleted ? SetStatus.Completed : SetStatus.Live;
            newSet.CreatedAt = DateTime.UtcNow;

            if (match.Status == MatchStatus.Upcoming)
            {
                match.Status    = MatchStatus.Live;
                match.UpdatedAt = DateTime.UtcNow;
                _matchRepo.Update(match);
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
                        string.Format(AppConstants.MatchSetAlreadyExists, 1, match.Id));
                return;
            }

            if (match.MatchSets.Count >= match.TotalSets)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.MaxSetsReached, match.TotalSets));

            int next = match.MatchSets.Count + 1;
            if (next > 1)
            {
                var prev = match.MatchSets.FirstOrDefault(s => s.SetNumber == next - 1);
                if (prev != null && prev.Status != SetStatus.Completed)
                    throw new UnprocessableEntityException(
                        string.Format(AppConstants.PreviousSetNotCompleted, next - 1, next));
            }
        }

        private static void ValidateSetsForResult(Match match)
        {
            if (!match.MatchSets.Any())
                throw new UnprocessableEntityException(AppConstants.NoScoreSubmitted);
            if (match.MatchSets.Any(s => s.Status == SetStatus.Live))
                throw new UnprocessableEntityException(AppConstants.AllSetsNotCompleted);
            if (match.TotalSets > 0 &&
                match.MatchSets.Count(s => s.Status == SetStatus.Completed) < match.TotalSets)
                throw new UnprocessableEntityException(AppConstants.AllSetsNotCompleted);
        }

        private static (int? winner, int? loser) DetermineWinnerAndLoser(Match match)
        {
            int sideAWins = match.MatchSets.Count(s => s.ScoreA > s.ScoreB);
            int sideBWins = match.MatchSets.Count(s => s.ScoreB > s.ScoreA);

            if (sideAWins == sideBWins)
                throw new UnprocessableEntityException(AppConstants.DrawNotAllowed);

            bool aWins = sideAWins > sideBWins;
            return (
                winner: aWins ? match.SideAId : match.SideBId,
                loser:  aWins ? match.SideBId : match.SideAId
            );
        }

        private static int GetLastRound(List<Match> allMatches) =>
            allMatches.Max(m => m.RoundNumber);

        private static bool IsOddBracket(List<Match> allMatches, int lastRound) =>
            allMatches.Any(m => m.RoundNumber == lastRound && m.BracketPosition == 1);

        private static bool IsFinal(Match match, int lastRound) =>
            match.RoundNumber == lastRound && match.BracketPosition == 0;

        private static bool IsByeMatch(Match match, int lastRound) =>
            match.RoundNumber == lastRound && match.BracketPosition == 1;

        private static bool IsSemiFinal(Match match, int lastRound, bool isOddBracket) =>
            isOddBracket && match.RoundNumber == lastRound - 1 && match.BracketPosition == 0;

        private async Task<MatchResultResponse> SubmitResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new ConflictException(string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            ValidateSetsForResult(match);

            var (winnerId, loserId) = DetermineWinnerAndLoser(match);

            var result = new Result
            {
                MatchId   = matchId,
                WinnerId  = winnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _matchRepo.AddResultAsync(result);

            match.Status    = MatchStatus.Completed;
            match.UpdatedAt = DateTime.UtcNow;
            _matchRepo.Update(match);

            var allMatches  = (await _matchRepo.GetByCategoryAsync(match.EventCategoryId, null)).ToList();
            int lastRound   = GetLastRound(allMatches);
            bool oddBracket = IsOddBracket(allMatches, lastRound);

            if (IsFinal(match, lastRound))
            {
            }
            else if (IsByeMatch(match, lastRound))
            {
                await AdvanceByeMatchWinnerAsync(match, winnerId, allMatches, lastRound);
            }
            else if (IsSemiFinal(match, lastRound, oddBracket))
            {
                await AdvanceSemiFinalAsync(match, winnerId, loserId, allMatches);
            }
            else
            {
                await AdvanceRegularWinnerAsync(match, winnerId, allMatches, lastRound, oddBracket);
            }

            var allCompleted = await _matchRepo.AllMatchesCompletedAsync(match.EventCategoryId);
            if (allCompleted)
                await _matchRepo.UpdateEventStatusAsync(match.EventCategoryId, EventStatus.Completed);

            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchResultResponse>(result);
        }

        private async Task AdvanceRegularWinnerAsync(Match completedMatch, int? winnerId, List<Match> allMatches, int lastRound, bool oddBracket)
        {
            var currentRoundMatches = allMatches
                .Where(m => m.RoundNumber == completedMatch.RoundNumber)
                .OrderBy(m => m.BracketPosition)
                .ToList();

            int positionInRound = currentRoundMatches.FindIndex(m => m.Id == completedMatch.Id);
            if (positionInRound < 0) return;

            var nextRoundMatches = allMatches
                .Where(m => m.RoundNumber == completedMatch.RoundNumber + 1
                         && !IsByeMatch(m, lastRound)
                         && !IsFinal(m, lastRound))
                .OrderBy(m => m.BracketPosition)
                .ToList();

            if (!nextRoundMatches.Any()) return;

            int nextMatchIndex = positionInRound / 2;
            if (nextMatchIndex >= nextRoundMatches.Count) return;

            var nextMatch = nextRoundMatches[nextMatchIndex];

            if (positionInRound % 2 == 0)
                nextMatch.SideAId = winnerId;
            else
                nextMatch.SideBId = winnerId;

            nextMatch.UpdatedAt = DateTime.UtcNow;
            _matchRepo.Update(nextMatch);
            await _matchRepo.SaveChangesAsync();
        }

        private async Task AdvanceSemiFinalAsync(Match semiFinal, int? winnerId, int? loserId, List<Match> allMatches)
        {
            int nextRound = semiFinal.RoundNumber + 1;

            var finalMatch = allMatches.FirstOrDefault(m =>
                m.RoundNumber == nextRound && m.BracketPosition == 0);

            var byeMatch = allMatches.FirstOrDefault(m =>
                m.RoundNumber == nextRound && m.BracketPosition == 1);

            if (finalMatch is not null)
            {
                finalMatch.SideAId   = winnerId;
                finalMatch.UpdatedAt = DateTime.UtcNow;
                _matchRepo.Update(finalMatch);
            }

            if (byeMatch is not null)
            {
                byeMatch.SideBId   = loserId;
                byeMatch.UpdatedAt = DateTime.UtcNow;
                _matchRepo.Update(byeMatch);
            }

            await _matchRepo.SaveChangesAsync();
        }

        private async Task AdvanceByeMatchWinnerAsync(Match byeMatch, int? winnerId, List<Match> allMatches, int lastRound)
        {
            var finalMatch = allMatches.FirstOrDefault(m =>
                m.RoundNumber == byeMatch.RoundNumber && IsFinal(m, lastRound));

            if (finalMatch is null) return;

            finalMatch.SideBId   = winnerId;
            finalMatch.UpdatedAt = DateTime.UtcNow;
            _matchRepo.Update(finalMatch);
            await _matchRepo.SaveChangesAsync();
        }
    }
}