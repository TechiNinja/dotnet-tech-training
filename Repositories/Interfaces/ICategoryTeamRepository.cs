using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ICategoryTeamRepository: IGenericRepository<Team>
    {
        Task<List<Team>> GetTeamsByCategoryAsync(int categoryId);
    }
}
