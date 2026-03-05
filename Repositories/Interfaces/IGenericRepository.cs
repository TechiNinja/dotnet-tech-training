using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T: class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
