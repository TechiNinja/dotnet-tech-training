using AutoMapper;
using SportsManagementApp.Constants;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services
{
    public class GenericService<TEntity, TResponseDto> : IGenericService<TResponseDto>
        where TEntity : class
    {
        protected readonly IGenericRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<TResponseDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException(
                    string.Format(StringConstant.EntityNotFound, typeof(TEntity).Name, id));
            return _mapper.Map<TResponseDto>(entity);
        }

        public virtual async Task<IEnumerable<TResponseDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TResponseDto>>(entities);
        }
    }
}