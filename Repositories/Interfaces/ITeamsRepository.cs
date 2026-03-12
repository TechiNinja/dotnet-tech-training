using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ITeamsRepository : IGenericRepository<Team>
    {
        Task<IEnumerable<Team>> GetByCategoryAsync(int catId);
        Task DeleteAllByCategoryAsync(int catId);
        Task<List<MyTeamDto>> GetUserTeamsAsync(int userId);
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
        Task AddTeamAsync(Team team);
        Task<List<TeamResponseDto>> GetTeamsByFilterAsync(Expression<Func<Team, bool>> predicate, Expression<Func<Team, TeamResponseDto>> projection);
    }
}