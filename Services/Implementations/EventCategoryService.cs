using AutoMapper;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services
{
    public class EventCategoryService : GenericService<EventCategory>, IEventCategoryService
    {
        private readonly IEventCategoryRepository _eventCategoryRepo;

        public EventCategoryService(
            IEventCategoryRepository eventCategoryRepo,
            IMapper mapper)
            : base(eventCategoryRepo, mapper)
        {
            _eventCategoryRepo = eventCategoryRepo;
        }

        public override async Task<TDto> GetByIdAsync<TDto>(int id)
        {
            var category = await _eventCategoryRepo.GetByIdWithDetailsAsync(id)
                ?? throw new NotFoundException(string.Format(StringConstant.CategoryNotFound, id));
            return _mapper.Map<TDto>(category);
        }
    }
}