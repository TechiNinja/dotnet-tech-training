using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ITeamsRepository : IGenericRepository<Team>
    {
        Task AddTeamAsync(Team team);
        Task<List<TeamResponseDto>> GetTeamsByFilterAsync(Expression<Func<Team, bool>> predicate, Expression<Func<Team, TeamResponseDto>> projection);
    }
}
