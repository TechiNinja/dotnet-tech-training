namespace SportsManagementApp.Services.Interfaces
{
    public interface IGenericService<TEntity> where TEntity : class
    {
        Task<TDto> GetByIdAsync<TDto>(int id);
        Task<IEnumerable<TDto>> GetAllAsync<TDto>();
    }
}