using System.Linq.Expressions;
using SportsManagementApp.Repositories.Specifications;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<List<TDto>> GetAllAsync<TDto>(Expression<Func<T, bool>> predicate, Expression<Func<T, TDto>> projection);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(ISpecification<T> spec);
        Task AddAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}