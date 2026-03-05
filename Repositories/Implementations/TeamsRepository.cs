using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class TeamsRepository: GenericRepository<Team>, ITeamsRepository
    {
        public TeamsRepository(AppDbContext context): base(context) { }

        public async Task<List<MyTeamDto>> GetUserTeamsAsync(int userId)
        {
            return await _context.TeamMembers
                .Where(member => member.UserId == userId)
                .Select(member => new MyTeamDto
                {
                    TeamId = member.TeamId,
                    TeamName = member.Team != null ? member.Team.Name : "N/A",
                    Category = member.Team != null && member.Team.EventCategory != null
                        ? $"{member.Team.EventCategory.Gender} {member.Team.EventCategory.Format}"
                        : "N/A",
                    EventName = member.Team != null
                        && member.Team.EventCategory != null
                        && member.Team.EventCategory.Event != null
                            ? member.Team.EventCategory.Event.Name
                            : "N/A",
                })
                .ToListAsync();
        }

        public async Task<List<Team>> GetTeamsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Include(team => team.Members)
                .ThenInclude(member => member.User)
                .Where(team => team.EventCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
