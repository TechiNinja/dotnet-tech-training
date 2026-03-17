using AutoMapper;
using SportsManagementApp.Constants;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Services.Strategies;

namespace SportsManagementApp.Services
{
    public class FixtureService : IFixtureService
    {
        private static readonly TimeOnly DayStart    = new(8, 0);
        private static readonly TimeOnly DayEnd      = new(17, 0);
        private const           int      SlotMinutes = 60;

        private readonly IEventCategoryRepository _categoryRepo;
        private readonly IMatchRepository         _matchRepo;
        private readonly IFixtureStrategy         _fixtureStrategy;
        private readonly IMapper                  _mapper;

        public FixtureService(
            IEventCategoryRepository categoryRepo,
            IMatchRepository         matchRepo,
            IFixtureStrategy         fixtureStrategy,
            IMapper                  mapper)
        {
            _categoryRepo    = categoryRepo;
            _matchRepo       = matchRepo;
            _fixtureStrategy = fixtureStrategy;
            _mapper          = mapper;
        }

        public async Task<IEnumerable<FixtureResponseDto>> GenerateFixturesAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            if (category.Matches.Any())
                throw new ConflictException(StringConstant.FixturesAlreadyExist);

            var matches = _fixtureStrategy.Generate(ExtractSideIds(category), catId);

            AssignSlots(matches, category.Event!);

            foreach (var match in matches)
                await _matchRepo.AddAsync(match);

            var created = await _matchRepo.GetByCategoryAsync(catId, null);
            return FixtureMapper.MapFixtures(created, category, _mapper);
        }

        public async Task<IEnumerable<FixtureResponseDto>> GetFixturesAsync(int catId, string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) && !Enum.TryParse<MatchStatus>(status, true, out _))
                throw new BadRequestException(string.Format(StringConstant.InvalidMatchStatus, status));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            var matches = await _matchRepo.GetByCategoryAsync(catId, status);
            return FixtureMapper.MapFixtures(matches, category, _mapper);
        }

        public async Task<FixtureResponseDto> RescheduleAsync(int catId, RescheduleRequestDto request)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            var match = category.Matches.FirstOrDefault(m => m.Id == request.MatchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, request.MatchId));

            if (match.Status == MatchStatus.Completed)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.MatchAlreadyCompleted, request.MatchId));

            var eventStart = category.Event!.StartDate.ToDateTime(DayStart);
            var eventEnd   = category.Event.EndDate.ToDateTime(DayEnd);

            if (request.NewStartDateTime < eventStart || request.NewStartDateTime > eventEnd)
                throw new BadRequestException(
                    string.Format(StringConstant.RescheduleOutsideEventDates,
                        category.Event.StartDate, category.Event.EndDate));

            var delay = request.NewStartDateTime - match.MatchDateTime;

            var affected = category.Matches
                .Where(m => m.Status != MatchStatus.Completed && m.MatchDateTime >= match.MatchDateTime)
                .OrderBy(m => m.MatchDateTime)
                .ToList();

            if (affected.Last().MatchDateTime.Add(delay) > eventEnd)
                throw new BadRequestException(StringConstant.ReschedulePushesMatchesBeyondEventEnd);

            var now = DateTime.UtcNow;
            foreach (var m in affected)
            {
                m.MatchDateTime = m.MatchDateTime.Add(delay);
                m.UpdatedAt     = now;
            }

            await _matchRepo.SaveChangesAsync();

            var updated = await _matchRepo.GetByIdWithSetsAndResultAsync(request.MatchId);
            return FixtureMapper.MapFixtures(new[] { updated! }, category, _mapper).First();
        }

        public async Task DeleteFixturesAsync(int catId)
        {
            if (!await _categoryRepo.ExistsAsync(c => c.Id == catId))
                throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));

            await _matchRepo.DeleteAllByCategoryAsync(catId);
        }

        private static void AssignSlots(IEnumerable<Match> matches, Event evt)
        {
            var slot = evt.StartDate.ToDateTime(DayStart);
            foreach (var match in matches)
            {
                match.MatchDateTime = slot;
                match.MatchVenue    = evt.EventVenue;
                slot                = AdvanceSlot(slot, evt.EndDate);
            }
        }

        private static DateTime AdvanceSlot(DateTime current, DateOnly endDate)
        {
            var next = current.AddMinutes(SlotMinutes);

            if (TimeOnly.FromDateTime(next) >= DayEnd)
                next = next.Date.AddDays(1).Add(DayStart.ToTimeSpan());

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
                return teams.OrderBy(_ => Guid.NewGuid()).Select(t => (int?)t.Id).ToList();
            }

            var participants = category.EventRegistrations.ToList();
            if (participants.Count < 2)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.NotEnoughParticipants, participants.Count));
            return participants.OrderBy(_ => Guid.NewGuid()).Select(p => (int?)p.UserId).ToList();
        }
    }
}