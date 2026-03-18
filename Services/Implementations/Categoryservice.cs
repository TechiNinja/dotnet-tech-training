using AutoMapper;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services
{
    public class CategoryService : GenericService<EventCategory, EventCategoryResponseDto>, ICategoryService
    {
        private readonly IEventCategoryRepository _categoryRepo;
        private readonly IMatchRepository _matchRepo;

        public CategoryService(
            IEventCategoryRepository categoryRepo,
            IMatchRepository matchRepo,
            IMapper mapper)
            : base(categoryRepo, mapper)
        {
            _categoryRepo = categoryRepo;
            _matchRepo = matchRepo;
        }

        public override async Task<EventCategoryResponseDto> GetByIdAsync(int catId)
        {
            var category = await _categoryRepo.GetByIdWithDetailsAsync(catId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, catId));
            return _mapper.Map<EventCategoryResponseDto>(category);
        }

        public async Task<FixtureResponseDto> GetMatchByIdAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(StringConstant.MatchNotFound, matchId));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(match.EventCategoryId)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, match.EventCategoryId));

            return FixtureMappingHelper.MapFixtures(new[] { match }, category, _mapper).First();
        }
    }
}