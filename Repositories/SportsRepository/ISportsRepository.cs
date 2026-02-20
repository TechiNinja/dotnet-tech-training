using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.SportsRepository
{
    public interface ISportRepository
    {
        Task<Sport> AddSport(Sport sport);
        Task<List<Sport>> SearchSports(int? id, string? name);
        Task<Sport?> GetSportByName(string name);
        Task<bool> Exists(int id);
      
    }
}
