using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;
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
        private readonly IMapper          _mapper;

        public MatchService(IMatchRepository matchRepo, IMapper mapper)
        {
            _matchRepo = matchRepo;
            _mapper    = mapper;
        }

        public async Task<MatchSetResponse> UpdateSetAsync(int matchId, MatchSetRequest request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            var liveSet = match.MatchSets.FirstOrDefault(s => s.Status == SetStatus.Live);

            if (liveSet != null)
            {
                liveSet.ScoreA    = request.ScoreA;
                liveSet.ScoreB    = request.ScoreB;
                liveSet.UpdatedAt = DateTime.UtcNow;

                if (request.IsCompleted)
                    liveSet.Status = SetStatus.Completed;

                _matchRepo.UpdateSet(liveSet);
                await _matchRepo.SaveChangesAsync();

                return _mapper.Map<MatchSetResponse>(liveSet);
            }

            if (match.TotalSets == 0)
            {
                if (match.MatchSets.Any())
                    throw new ConflictException(string.Format(AppConstants.MatchSetAlreadyExists, 1, matchId));
            }
            else
            {
                if (match.MatchSets.Count >= match.TotalSets)
                    throw new UnprocessableEntityException(string.Format(AppConstants.MaxSetsReached, match.TotalSets));

                int nextSetNumber = match.MatchSets.Count + 1;
                if (nextSetNumber > 1)
                {
                    var previousSet = match.MatchSets.FirstOrDefault(s => s.SetNumber == nextSetNumber - 1);
                    if (previousSet != null && previousSet.Status != SetStatus.Completed)
                        throw new UnprocessableEntityException(
                            string.Format(AppConstants.PreviousSetNotCompleted, nextSetNumber - 1, nextSetNumber));
                }
            }

            int setNumber = match.MatchSets.Any()
                ? match.MatchSets.Max(s => s.SetNumber) + 1
                : 1;

            var newSet = new MatchSet
            {
                MatchId   = matchId,
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

            return _mapper.Map<MatchSetResponse>(newSet);
        }

        public async Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            return _mapper.Map<IEnumerable<MatchSetResponse>>(match.MatchSets.OrderBy(s => s.SetNumber));
        }

        public async Task<MatchResultResponse> SubmitResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new ConflictException(string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            if (!match.MatchSets.Any())
                throw new UnprocessableEntityException(AppConstants.NoScoreSubmitted);

            if (match.MatchSets.Any(s => s.Status == SetStatus.Live))
                throw new UnprocessableEntityException(AppConstants.AllSetsNotCompleted);

            if (match.TotalSets > 0)
            {
                int completedSets = match.MatchSets.Count(s => s.Status == SetStatus.Completed);
                if (completedSets < match.TotalSets)
                    throw new UnprocessableEntityException(AppConstants.AllSetsNotCompleted);
            }

            int sideAWins = match.MatchSets.Count(s => s.ScoreA > s.ScoreB);
            int sideBWins = match.MatchSets.Count(s => s.ScoreB > s.ScoreA);

            if (sideAWins == sideBWins)
                throw new UnprocessableEntityException(AppConstants.DrawNotAllowed);

            int? winnerId = sideAWins > sideBWins ? match.SideAId : match.SideBId;

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
            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchResultResponse>(result);
        }

        public async Task<MatchResultResponse> GetResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Result == null)
                throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            return _mapper.Map<MatchResultResponse>(match.Result);
        }
    }
}