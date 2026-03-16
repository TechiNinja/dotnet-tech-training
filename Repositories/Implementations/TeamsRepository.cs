using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
    {
        public TeamsRepository(AppDbContext context) : base(context) { }

        public async Task<List<TeamResponseDto>> GetTeamsByFilterAsync(Expression<Func<Team, bool>> predicate, Expression<Func<Team, TeamResponseDto>> projection)
        {
            return await GetAllAsync(predicate, projection);
        }
    }
}
