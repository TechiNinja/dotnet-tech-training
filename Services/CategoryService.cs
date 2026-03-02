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
    public class CategoryService : ICategoryService
    {
        private readonly IEventCategoryRepository _categoryRepo;
        private readonly IMatchRepository         _matchRepo;
        private readonly IMapper                  _mapper;

        public CategoryService(
            IEventCategoryRepository categoryRepo,
            IMatchRepository         matchRepo,
            IMapper                  mapper)
        {
            _categoryRepo = categoryRepo;
            _matchRepo    = matchRepo;
            _mapper       = mapper;
        }

        public async Task<CategoryResponse> GetByIdAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<IEnumerable<FixtureResponse>> GenerateFixturesAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            if (category.Matches.Any())
                throw new ConflictException(AppConstants.FixturesAlreadyExist);

            var matches = category.Format == MatchFormat.Singles
                ? GenerateSinglesFixtures(category)
                : GenerateDoublesFixtures(category);

            foreach (var match in matches)
                await _matchRepo.AddAsync(match);

            await _matchRepo.SaveChangesAsync();

            var created = await _matchRepo.GetByCategoryAsync(catId, null);
            return MapFixtures(created, category);
        }

        public async Task<IEnumerable<FixtureResponse>> GetFixturesAsync(int catId, string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) &&
                !Enum.TryParse<MatchStatus>(status, true, out _))
                throw new BadRequestException(string.Format(AppConstants.InvalidMatchStatus, status));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            var matches = await _matchRepo.GetByCategoryAsync(catId, status);
            return MapFixtures(matches, category);
        }

        public async Task<FixtureResponse> GetMatchByIdAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(match.EventCategoryId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, match.EventCategoryId));

            return MapFixtures(new[] { match }, category).First();
        }

        public async Task DeleteFixturesAsync(int catId)
        {
            await EnsureCategoryExistsAsync(catId);
            await _matchRepo.DeleteAllByCategoryAsync(catId);
            await _matchRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<FixtureResponse>> BulkScheduleAsync(int catId, BulkScheduleRequest request)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));

            foreach (var item in request.Schedules)
            {
                var match = await _matchRepo.GetByIdWithSetsAndResultAsync(item.MatchId)
                    ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, item.MatchId));

                if (match.EventCategoryId != catId)
                    throw new BadRequestException(string.Format(AppConstants.MatchNotFound, item.MatchId));

                if (match.Status == MatchStatus.Completed)
                    throw new UnprocessableEntityException(string.Format(AppConstants.MatchAlreadyCompleted, item.MatchId));

                if (await _matchRepo.HasOverlapAsync(catId, item.MatchDateTime, item.MatchId))
                    throw new ConflictException(AppConstants.ScheduleTimeOverlap);

                match.MatchDateTime = item.MatchDateTime;
                match.MatchVenue    = category.Event?.EventVenue ?? string.Empty;
                match.UpdatedAt     = DateTime.UtcNow;

                _matchRepo.Update(match);
            }

            await _matchRepo.SaveChangesAsync();

            var updated = await _matchRepo.GetByCategoryAsync(catId, null);
            return MapFixtures(updated, category);
        }

        private List<Match> GenerateSinglesFixtures(EventCategory category)
        {
            var participants = category.EventRegistrations.ToList();

            if (participants.Count < 2)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.NotEnoughParticipants, participants.Count));

            var shuffled = participants.OrderBy(_ => Guid.NewGuid()).Select(p => (int?)p.UserId).ToList();

            return category.Event?.TournamentType == TournamentType.RoundRobin
                ? GenerateRoundRobin(shuffled, category.Id)
                : GenerateKnockout(shuffled, category.Id);
        }

        private List<Match> GenerateDoublesFixtures(EventCategory category)
        {
            var teams = category.Teams.ToList();

            if (teams.Count < 2)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.NotEnoughTeams, teams.Count));

            var shuffled = teams.OrderBy(_ => Guid.NewGuid()).Select(t => (int?)t.Id).ToList();

            return category.Event?.TournamentType == TournamentType.RoundRobin
                ? GenerateRoundRobin(shuffled, category.Id)
                : GenerateKnockout(shuffled, category.Id);
        }

        private static List<Match> GenerateKnockout(List<int?> sideIds, int catId)
        {
            var matches     = new List<Match>();
            var matchNumber = 1;
            var bracketPos  = 1;

            int totalSlots = NextPowerOfTwo(sideIds.Count);
            while (sideIds.Count < totalSlots)
                sideIds.Add(null);

            var currentRound = sideIds;
            var roundNumber  = 1;

            while (currentRound.Count > 1)
            {
                var nextRound = new List<int?>();

                for (int i = 0; i < currentRound.Count; i += 2)
                {
                    var sideA = currentRound[i];
                    var sideB = i + 1 < currentRound.Count ? currentRound[i + 1] : null;

                    matches.Add(new Match
                    {
                        EventCategoryId = catId,
                        SideAId         = sideA,
                        SideBId         = sideB,
                        RoundNumber     = roundNumber,
                        MatchNumber     = matchNumber++,
                        BracketPosition = bracketPos++,
                        Status          = MatchStatus.Upcoming,
                        MatchVenue      = string.Empty,
                        MatchDateTime   = default,
                        CreatedAt       = DateTime.UtcNow
                    });

                    if (sideA.HasValue && !sideB.HasValue)
                        nextRound.Add(sideA);
                    else if (!sideA.HasValue && sideB.HasValue)
                        nextRound.Add(sideB);
                    else
                        nextRound.Add(null);
                }

                currentRound = nextRound;
                roundNumber++;
            }

            return matches;
        }

        private static List<Match> GenerateRoundRobin(List<int?> sideIds, int catId)
        {
            var matches     = new List<Match>();
            var matchNumber = 1;
            var bracketPos  = 1;
            var roundNumber = 1;
            var sides       = sideIds.ToList();

            if (sides.Count % 2 != 0)
                sides.Add(null);

            int totalRounds = sides.Count - 1;
            int halfSize    = sides.Count / 2;

            for (int round = 0; round < totalRounds; round++)
            {
                for (int i = 0; i < halfSize; i++)
                {
                    var sideA = sides[i];
                    var sideB = sides[sides.Count - 1 - i];

                    if (!sideA.HasValue && !sideB.HasValue) continue;

                    matches.Add(new Match
                    {
                        EventCategoryId = catId,
                        SideAId         = sideA,
                        SideBId         = sideB,
                        RoundNumber     = roundNumber,
                        MatchNumber     = matchNumber++,
                        BracketPosition = bracketPos++,
                        Status          = MatchStatus.Upcoming,
                        MatchVenue      = string.Empty,
                        MatchDateTime   = default,
                        CreatedAt       = DateTime.UtcNow
                    });
                }

                var last = sides[sides.Count - 1];
                sides.RemoveAt(sides.Count - 1);
                sides.Insert(1, last);
                roundNumber++;
            }

            return matches;
        }

        private static int NextPowerOfTwo(int n)
        {
            int power = 1;
            while (power < n) power *= 2;
            return power;
        }

        private async Task EnsureCategoryExistsAsync(int catId)
        {
            if (!await _categoryRepo.ExistsAsync(c => c.Id == catId))
                throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, catId));
        }

        private IEnumerable<FixtureResponse> MapFixtures(IEnumerable<Match> matches, EventCategory category)
        {
            var teamNames = category.Teams.ToDictionary(t => t.Id, t => t.Name);
            var userNames = category.EventRegistrations
                .Where(r => r.User != null)
                .ToDictionary(r => r.UserId, r => r.User!.FullName);

            return matches.Select(m =>
            {
                var response       = _mapper.Map<FixtureResponse>(m);
                response.SideAName = ResolveSideName(m.SideAId, category.Format, teamNames, userNames);
                response.SideBName = ResolveSideName(m.SideBId, category.Format, teamNames, userNames);
                return response;
            });
        }

        private static string ResolveSideName(
            int? sideId,
            MatchFormat format,
            Dictionary<int, string> teamNames,
            Dictionary<int, string> userNames)
        {
            if (!sideId.HasValue) return "BYE";

            if (format == MatchFormat.Doubles && teamNames.TryGetValue(sideId.Value, out var teamName))
                return teamName;

            if (userNames.TryGetValue(sideId.Value, out var userName))
                return userName;

            return $"#{sideId}";
        }
    }
}