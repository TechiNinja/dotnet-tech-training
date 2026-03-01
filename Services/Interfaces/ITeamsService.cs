using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.DTOs.TeamManagement;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<List<MyTeamDto>> GetUserTeamsAsync(int userId);
        Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request);
        Task<List<TeamResponseDto>> GetTeamsByCategoryAsync(int categoryId);
    }
}
