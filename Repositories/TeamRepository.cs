using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Team>> GetByCategoryAsync(int catId) =>
            await _context.Teams
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Where(t => t.EventCategoryId == catId)
                .AsNoTracking()
                .ToListAsync();

        public async Task DeleteAllByCategoryAsync(int catId)
        {
            var teams = await _context.Teams
                .Where(t => t.EventCategoryId == catId)
                .ToListAsync();
            _context.Teams.RemoveRange(teams);
        }
    }
}