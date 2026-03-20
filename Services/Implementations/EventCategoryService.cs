using AutoMapper;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services
{
    public class EventCategoryService : GenericService<EventCategory>, IEventCategoryService
    {
        private readonly IEventCategoryRepository _eventCategoryRepo;
        private readonly IMatchRepository _matchRepo;

        public EventCategoryService(
            IEventCategoryRepository eventCategoryRepo,
            IMatchRepository matchRepo,
            IMapper mapper)
            : base(eventCategoryRepo, mapper)
        {
            _eventCategoryRepo = eventCategoryRepo;
            _matchRepo = matchRepo;
        }

        public override async Task<TDto> GetByIdAsync<TDto>(int id)
        {
            var category = await _eventCategoryRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, id));
            return _mapper.Map<TDto>(category);
        }

        public async Task<FixtureResponseDto> GetMatchByIdAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            var category = await _eventCategoryRepo.GetByIdWithDetailsAsync(match.EventCategoryId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, match.EventCategoryId));

            return FixtureMappingHelper.MapFixtures(new[] { match }, category, _mapper).First();
        }
    }
}