using System.Linq.Expressions;
using SportsManagementApp.Repositories.Specifications;

namespace SportsManagementApp.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository contract.
    /// Provides both expression-predicate and Specification overloads (DIP).
    /// Consumers depend only on this abstraction — never on EF Core directly.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?>             GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(ISpecification<T> spec);
        Task<bool>           ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<bool>           ExistsAsync(ISpecification<T> spec);
        Task                 AddAsync(T entity);
        void                 Update(T entity);
        void                 Delete(T entity);
        void                 DeleteRange(IEnumerable<T> entities);
        Task                 SaveChangesAsync();
    }
}
