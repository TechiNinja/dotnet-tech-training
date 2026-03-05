using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<IEnumerable<Team>> GetByCategoryAsync(int catId);
        Task                    DeleteAllByCategoryAsync(int catId);
    }
}
