using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
    {
        public TeamsRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Team>> GetByCategoryAsync(int catId) =>
            await _context.Teams
                .Include(t => t.Members).ThenInclude(m => m.User)
                .Where(t => t.EventCategoryId == catId)
                .AsNoTracking()
                .ToListAsync();

        public async Task DeleteAllByCategoryAsync(int catId) =>
            await _context.Teams
                .Where(t => t.EventCategoryId == catId)
                .ExecuteDeleteAsync();

        public async Task<List<MyTeamDto>> GetUserTeamsAsync(int userId) =>
            await _context.TeamMembers
                .Where(m => m.UserId == userId)
                .Select(m => new MyTeamDto
                {
                    TeamId    = m.TeamId,
                    TeamName  = m.Team != null ? m.Team.Name : "N/A",
                    Category  = m.Team != null && m.Team.EventCategory != null
                        ? $"{m.Team.EventCategory.Gender} {m.Team.EventCategory.Format}" : "N/A",
                    EventName = m.Team != null && m.Team.EventCategory != null && m.Team.EventCategory.Event != null
                        ? m.Team.EventCategory.Event.Name : "N/A",
                })
                .ToListAsync();

        public async Task<List<Team>> GetTeamsByCategoryAsync(int categoryId) =>
            await _dbSet.Include(t => t.Members).ThenInclude(m => m.User)
                .Where(t => t.EventCategoryId == categoryId)
                .ToListAsync();

        public async Task<List<TeamResponseDto>> GetTeamsByFilterAsync(Expression<Func<Team, bool>> predicate, Expression<Func<Team, TeamResponseDto>> projection)
        {
            return await GetAllAsync(predicate, projection);
        }

        public async Task AddTeamAsync(Team team)
        {
            await _dbSet.AddAsync(team);
            await _context.SaveChangesAsync();
        }
    }
}