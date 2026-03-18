using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ISportRepository: IGenericRepository<Sport>
    {
        Task<bool> SportExistsAsync(string name);
    }
}