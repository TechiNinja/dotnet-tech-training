using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data.DTOs.SportManagement;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SportRepository : GenericRepository<Sport>, ISportRepository
    {
        public SportRepository(AppDbContext context) : base(context) { }

        public async Task<bool> SportExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(sport => sport.Name == name);
        }

        public async Task<Sport> CreateSportAsync(string name)
        {
            var sport = new Sport
            {
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(sport);
            return sport;
        }

        public async Task<List<SportResponseDto>> GetSportsAsync(Expression<Func<Sport, bool>> predicate, Expression<Func<Sport, SportResponseDto>> projection)
        {
            return await GetAllAsync(predicate, projection);
        }

        public async Task<Sport?> GetSportByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task UpdateSportAsync(Sport sport)
        {
            await UpdateAsync(sport);
        }
    }
}
