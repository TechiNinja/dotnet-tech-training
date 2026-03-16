﻿using Microsoft.EntityFrameworkCore;
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
        public async Task<T?> GetByIdWithIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAllListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<TDto>> GetAllAsync<TDto>(Expression<Func<T, bool>> predicate, Expression<Func<T, TDto>> projection)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .Select(projection)
                .ToListAsync();
        }

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

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}