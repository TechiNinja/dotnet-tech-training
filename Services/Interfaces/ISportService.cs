using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ISportService
    {
        Task<Sport> CreateSportAsync(CreateSportDto createSport);
        Task<IEnumerable<Sport>> GetSportsAsync();
    }
}
