using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<List<TeamResponseDto>> GetTeamsAsync(TeamFilterDto filter);
        Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request);
    }
}
