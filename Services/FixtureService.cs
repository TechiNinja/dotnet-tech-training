using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Services.Strategies;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services
{
    public class FixtureService : IFixtureService
    {
        private readonly IEventCategoryRepository _categoryRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IFixtureStrategyResolver _strategyResolver;
        private readonly IMapper _mapper;

        public FixtureService(
            IEventCategoryRepository categoryRepo,
            IMatchRepository matchRepo,
            IFixtureStrategyResolver strategyResolver,
            IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _matchRepo = matchRepo;
            _strategyResolver = strategyResolver;
            _mapper           = mapper;
        }

        public async Task<IEnumerable<FixtureResponse>> GenerateFixturesAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            if (category.Matches.Any())
                throw new ConflictException(AppConstants.FixturesAlreadyExist);

            var sideIds  = ExtractSideIds(category);
            var strategy = _strategyResolver.Resolve(category.Event?.TournamentType ?? TournamentType.Knockout);
            var matches  = strategy.Generate(sideIds, catId);

            foreach (var match in matches)
                await _matchRepo.AddAsync(match);

            await _matchRepo.SaveChangesAsync();

            var created = await _matchRepo.GetByCategoryAsync(catId, null);
            return FixtureMapper.MapFixtures(created, category, _mapper);
        }

        public async Task<IEnumerable<FixtureResponse>> GetFixturesAsync(int catId, string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) &&
                !Enum.TryParse<MatchStatus>(status, true, out _))
                throw new BadRequestException(string.Format(AppConstants.InvalidMatchStatus, status));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            var matches = await _matchRepo.GetByCategoryAsync(catId, status);
            return FixtureMapper.MapFixtures(matches, category, _mapper);
        }

        public async Task<IEnumerable<FixtureResponse>> BulkScheduleAsync(int catId, List<MatchScheduleItem> schedules)        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            foreach (var item in schedules)
            {
                var match = await _matchRepo.GetByIdWithSetsAndResultAsync(item.MatchId)
                    ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, item.MatchId));

                if (match.EventCategoryId != catId)
                    throw new BadRequestException(string.Format(AppConstants.MatchNotFound, item.MatchId));

                if (match.Status == MatchStatus.Completed)
                    throw new UnprocessableEntityException(
                        string.Format(AppConstants.MatchAlreadyCompleted, item.MatchId));

                if (await _matchRepo.HasOverlapAsync(catId, item.MatchDateTime, item.MatchId))
                    throw new ConflictException(AppConstants.ScheduleTimeOverlap);

                match.MatchDateTime = item.MatchDateTime;
                match.MatchVenue    = category.Event?.EventVenue ?? string.Empty;
                match.TotalSets     = item.TotalSets;
                match.UpdatedAt     = DateTime.UtcNow;
                _matchRepo.Update(match);
            }

            await _matchRepo.SaveChangesAsync();

            var updated = await _matchRepo.GetByCategoryAsync(catId, null);
            return FixtureMapper.MapFixtures(updated, category, _mapper);
        }

        public async Task DeleteFixturesAsync(int catId)
        {
            if (!await _categoryRepo.ExistsAsync(c => c.Id == catId))
                throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            await _matchRepo.DeleteAllByCategoryAsync(catId);
        }

        private static List<int?> ExtractSideIds(EventCategory category)
        {
            if (category.Format == MatchFormat.Doubles)
            {
                var teams = category.Teams.ToList();
                if (teams.Count < 2)
                    throw new UnprocessableEntityException(
                        string.Format(AppConstants.NotEnoughTeams, teams.Count));
                return teams.OrderBy(_ => Guid.NewGuid()).Select(t => (int?)t.Id).ToList();
            }

            var participants = category.EventRegistrations.ToList();
            if (participants.Count < 2)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.NotEnoughParticipants, participants.Count));
            return participants.OrderBy(_ => Guid.NewGuid()).Select(p => (int?)p.UserId).ToList();
        }
    }
}
