using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<T?> GetByIdWithIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetAllWithIncludesAsync(
            Expression<Func<T, bool>>? predicate = null,
            params Expression<Func<T, object>>[] includes);

        Task<List<TDto>> GetAllAsync<TDto>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TDto>> projection);

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}