using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ISportRepository
    {
        Task<bool> SportExistsAsync(string name);
        Task<Sport> CreateSportAsync(string name);
        Task<IEnumerable<Sport>> GetSportsAsync();
        Task<Sport?> GetSportByIdAsync(int id);
        Task UpdateSportAsync(Sport sport);
    }
}
