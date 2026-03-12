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
            liveSet.ScoreA    = request.ScoreA;
            liveSet.ScoreB    = request.ScoreB;
            liveSet.UpdatedAt = DateTime.UtcNow;
            if (request.IsCompleted) liveSet.Status = SetStatus.Completed;
            _matchRepo.UpdateSet(liveSet);
            await _matchRepo.SaveChangesAsync();
            return liveSet;
        }

        private async Task<MatchSet> CreateNextSetAsync(Match match, MatchSetRequest request)
        {
            ValidateNewSetAllowed(match);

            int setNumber = match.MatchSets.Any()
                ? match.MatchSets.Max(s => s.SetNumber) + 1
                : 1;

            var newSet = new MatchSet
            {
                MatchId   = match.Id,
                SetNumber = setNumber,
                ScoreA    = request.ScoreA,
                ScoreB    = request.ScoreB,
                Status    = request.IsCompleted ? SetStatus.Completed : SetStatus.Live,
                CreatedAt = DateTime.UtcNow
            };

            if (match.Status == MatchStatus.Upcoming)
            {
                match.Status    = MatchStatus.Live;
                match.UpdatedAt = DateTime.UtcNow;
                _matchRepo.Update(match);
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

        private static int? DetermineWinner(Match match)
        {
            int sideAWins = match.MatchSets.Count(s => s.ScoreA > s.ScoreB);
            int sideBWins = match.MatchSets.Count(s => s.ScoreB > s.ScoreA);
            if (sideAWins == sideBWins)
                throw new UnprocessableEntityException(AppConstants.DrawNotAllowed);
            return sideAWins > sideBWins ? match.SideAId : match.SideBId;
        }

        private async Task<MatchResultResponse> SubmitResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new ConflictException(string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            ValidateSetsForResult(match);

            int? winnerId = DetermineWinner(match);

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

            await AdvanceWinnerAsync(match, winnerId);

            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchResultResponse>(result);
        }

        private async Task AdvanceWinnerAsync(Match completedMatch, int? winnerId)
        {
            var nextBracketPos = (completedMatch.BracketPosition + 1) / 2;

            var nextMatch = await _matchRepo.GetByRoundAndBracketAsync(
                completedMatch.EventCategoryId,
                completedMatch.RoundNumber + 1,
                nextBracketPos);

            if (nextMatch == null) return;

            if (completedMatch.BracketPosition % 2 != 0)
                nextMatch.SideAId = winnerId;
            else
                nextMatch.SideBId = winnerId;

            nextMatch.UpdatedAt = DateTime.UtcNow;
            _matchRepo.Update(nextMatch);
        }
    }
}