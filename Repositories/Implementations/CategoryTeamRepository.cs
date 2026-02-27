using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class CategoryTeamRepository: GenericRepository<Team>, ICategoryTeamRepository
    {
        public CategoryTeamRepository(AppDbContext context) : base(context) { }

        public async Task<List<Team>> GetTeamsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Include(team => team.Members)
                .ThenInclude(member => member.User)
                .Where(team => team.EventCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
