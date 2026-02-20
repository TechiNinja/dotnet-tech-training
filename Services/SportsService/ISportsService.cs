using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;

namespace SportsManagementApp.Services
{
    public interface ISportService
    {
        Task<Sport> CreateSport(CreateSportDto dto);
        Task<IEnumerable<Sport>> SearchSports(int? id, string? name);
        
    }
}
