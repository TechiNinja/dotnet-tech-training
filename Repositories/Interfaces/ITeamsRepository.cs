using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ITeamsRepository: IGenericRepository<Team>
    {
        Task<List<MyTeamDto>> GetUserTeamsAsync(int userId);
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
    }
}
