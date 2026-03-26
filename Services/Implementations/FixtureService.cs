using AutoMapper;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Services.Strategies;

namespace SportsManagementApp.Services
{
    public class FixtureService : IFixtureService
    {

        private readonly IEventCategoryRepository _categoryRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IFixtureStrategy _fixtureStrategy;
        private readonly IMapper _mapper;

        public FixtureService(
            IEventCategoryRepository categoryRepo,
            IMatchRepository matchRepo,
            IFixtureStrategy fixtureStrategy,
            IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _matchRepo = matchRepo;
            _fixtureStrategy = fixtureStrategy;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FixtureResponseDto>> GenerateFixturesAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            if (category.Matches.Any())
                throw new ConflictException(StringConstant.FixturesAlreadyExist);

            var matches = _fixtureStrategy.Generate(ExtractSideIds(category), catId);

            AssignSlots(matches, category.Event!);

            await _matchRepo.AddRangeAsync(matches);
            await _matchRepo.SaveChangesAsync();

            var created = await _matchRepo.GetByCategoryAsync(catId, null);
            return FixtureMappingHelper.MapFixtures(created, category, _mapper);
        }

        public async Task<IEnumerable<FixtureResponseDto>> GetFixturesAsync(int catId, string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) && !Enum.TryParse<MatchStatus>(status, true, out _))
                throw new BadRequestException(string.Format(StringConstant.InvalidMatchStatus, status));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            var matches = await _matchRepo.GetByCategoryAsync(catId, status);
            return FixtureMappingHelper.MapFixtures(matches, category, _mapper);
        }

        public async Task DeleteFixturesAsync(int catId)
        {
            if (!await _categoryRepo.ExistsAsync(c => c.Id == catId))
                throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            await _matchRepo.DeleteAllByCategoryAsync(catId);
        }

        private static void AssignSlots(IEnumerable<Match> matches, Event evt)
        {
            var slot = evt.StartDate.ToDateTime(StringConstant.DayStart);
            foreach (var match in matches)
            {
                match.MatchDateTime = slot;
                match.MatchVenue = evt.EventVenue;
                slot = GetNextSlot(slot, evt.EndDate);
            }
        }

        private static DateTime GetNextSlot(DateTime current, DateOnly endDate)
        {
            var next = current.AddMinutes(StringConstant.SlotMinutes);

            if (TimeOnly.FromDateTime(next) >= StringConstant.DayEnd)
                next = next.Date.AddDays(1).Add(StringConstant.DayStart.ToTimeSpan());

            if (DateOnly.FromDateTime(next) > endDate)
                throw new UnprocessableEntityException(StringConstant.NotEnoughDaysToSchedule);

            return next;
        }

        private static List<int?> ExtractSideIds(EventCategory category)
        {
            if (category.Format == MatchFormat.Doubles)
            {
                var teams = category.Teams.ToList();
                if (teams.Count < 2)
                    throw new UnprocessableEntityException(
                        string.Format(StringConstant.NotEnoughTeams, teams.Count));
                return teams.OrderBy(_ => Random.Shared.Next()).Select(t => (int?)t.Id).ToList();
            }

            var participants = category.EventRegistrations.ToList();
            if (participants.Count < 2)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.NotEnoughParticipants, participants.Count));
            return participants.OrderBy(_ => Random.Shared.Next()).Select(p => (int?)p.UserId).ToList();
        }
    }
}