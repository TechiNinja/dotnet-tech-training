using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SportsManagementApp.Repositories.Implementations
{
    public class TeamsRepository: ITeamsRepository
    {
        private readonly AppDbContext _context;

        public TeamsRepository(AppDbContext context)
        {
            _context = context;
        }

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
    }
}
