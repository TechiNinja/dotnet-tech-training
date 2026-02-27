using SportsManagementApp.Data.DTOs.TeamManagement;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ICategoryTeamService
    {
        Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request);
        Task<List<TeamResponseDto>> GetTeamsByCategoryAsync(int categoryId);
    }
}
