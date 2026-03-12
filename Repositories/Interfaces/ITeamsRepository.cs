using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ITeamsRepository : IGenericRepository<Team>
    {
        Task<IEnumerable<Team>> GetByCategoryAsync(int catId);
        Task DeleteAllByCategoryAsync(int catId);
        Task<List<MyTeamDto>> GetUserTeamsAsync(int userId);
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
    }
}