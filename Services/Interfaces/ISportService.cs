using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ISportService
    {
        Task<Sport> CreateSportAsync(CreateSportDto createSport);
        Task<List<SportResponseDto>> GetSportsAsync(SportFilterDto filter);
        Task<Sport> UpdateSportAsync(int id, UpdateSportDto updateSport);
    }
}