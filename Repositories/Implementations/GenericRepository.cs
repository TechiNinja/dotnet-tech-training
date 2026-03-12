using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Repositories.Specifications;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(ISpecification<T> spec) =>
            await _dbSet.AsNoTracking().Where(spec.ToExpression()).ToListAsync();

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.AnyAsync(predicate);

        public async Task<bool> ExistsAsync(ISpecification<T> spec) =>
            await _dbSet.AnyAsync(spec.ToExpression());

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity) => _dbSet.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<int> SaveChangesAsync(bool returnCount) =>
            await _context.SaveChangesAsync();
    }
}