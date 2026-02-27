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

        public async Task<MatchSetResponse> StartSetAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            var liveSet = match.MatchSets.FirstOrDefault(s => s.Status == SetStatus.Live);
            if (liveSet != null)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.AnotherSetAlreadyLive, liveSet.SetNumber));

            if (match.TotalSets == 0)
            {
                if (match.MatchSets.Any())
                    throw new ConflictException(
                        string.Format(AppConstants.MatchSetAlreadyExists, 1, matchId));
            }
            else
            {
                if (match.MatchSets.Count >= match.TotalSets)
                    throw new UnprocessableEntityException(
                        string.Format(AppConstants.MaxSetsReached, match.TotalSets));

                int nextSetNumber = match.MatchSets.Count + 1;
                if (nextSetNumber > 1)
                {
                    var previousSet = match.MatchSets
                        .FirstOrDefault(s => s.SetNumber == nextSetNumber - 1);
                    if (previousSet != null && previousSet.Status != SetStatus.Completed)
                        throw new UnprocessableEntityException(
                            string.Format(AppConstants.PreviousSetNotCompleted,
                                nextSetNumber - 1, nextSetNumber));
                }
            }

            int setNumber = match.MatchSets.Any()
                ? match.MatchSets.Max(s => s.SetNumber) + 1
                : 1;

            var matchSet = new MatchSet
            {
                MatchId   = matchId,
                SetNumber = setNumber,
                ScoreA    = 0,
                ScoreB    = 0,
                Status    = SetStatus.Live,
                CreatedAt = DateTime.UtcNow
            };

            if (match.Status == MatchStatus.Upcoming)
            {
                match.Status    = MatchStatus.Live;
                match.UpdatedAt = DateTime.UtcNow;
                _matchRepo.Update(match);
            }

            match.MatchSets.Add(matchSet);
            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchSetResponse>(matchSet);
        }

        public async Task<MatchSetResponse> UpdateSetScoreAsync(
            int matchId, int setId, MatchSetRequest request)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            var set = await _matchRepo.GetSetByIdAsync(matchId, setId)
                ?? throw new NotFoundException(
                    string.Format(AppConstants.SetNotFound, setId, matchId));

            if (set.Status == SetStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.SetAlreadyCompleted, set.SetNumber));

            if (set.Status != SetStatus.Live)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.SetNotLive, set.SetNumber));

            set.ScoreA    = request.ScoreA;
            set.ScoreB    = request.ScoreB;
            set.UpdatedAt = DateTime.UtcNow;

            _matchRepo.UpdateSet(set);
            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchSetResponse>(set);
        }

        public async Task<MatchSetResponse> CompleteSetAsync(int matchId, int setId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            var set = await _matchRepo.GetSetByIdAsync(matchId, setId)
                ?? throw new NotFoundException(
                    string.Format(AppConstants.SetNotFound, setId, matchId));

            if (set.Status == SetStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.SetAlreadyCompleted, set.SetNumber));

            if (set.Status != SetStatus.Live)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.SetNotLive, set.SetNumber));

            set.Status    = SetStatus.Completed;
            set.UpdatedAt = DateTime.UtcNow;

            _matchRepo.UpdateSet(set);
            await _matchRepo.SaveChangesAsync();

            return _mapper.Map<MatchSetResponse>(set);
        }

        public async Task<IEnumerable<MatchSetResponse>> GetSetsAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            return _mapper.Map<IEnumerable<MatchSetResponse>>(
                match.MatchSets.OrderBy(s => s.SetNumber));
        }

        public async Task<MatchResultResponse> SubmitResultAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            if (match.Status == MatchStatus.Completed)
                throw new ConflictException(
                    string.Format(AppConstants.MatchAlreadyCompleted, matchId));

            if (!match.MatchSets.Any())
                throw new UnprocessableEntityException(AppConstants.NoScoreSubmitted);

            if (match.TotalSets > 0)
            {
                bool anyNotCompleted = match.MatchSets
                    .Any(s => s.Status != SetStatus.Completed);
                if (anyNotCompleted)
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
                throw new NotFoundException(
                    string.Format(AppConstants.MatchNotFound, matchId));

            return _mapper.Map<MatchResultResponse>(match.Result);
        }
    }
}