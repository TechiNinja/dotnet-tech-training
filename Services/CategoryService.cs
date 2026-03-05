using AutoMapper;
using SportsManagementApp.DTOs.Response;
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

        public async Task<FixtureResponse> GetMatchByIdAsync(int matchId)
        {
            var match = await _matchRepo.GetByIdWithSetsAndResultAsync(matchId)
                ?? throw new NotFoundException(string.Format(AppConstants.MatchNotFound, matchId));

            var category = await _categoryRepo.GetByIdWithDetailsAsync(match.EventCategoryId)
                ?? throw new NotFoundException(string.Format(AppConstants.CategoryNotFound, match.EventCategoryId));

            return FixtureMapper.MapFixtures(new[] { match }, category, _mapper).First();
        }
    }
}
