using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class TeamsService: ITeamsService
    {
        private readonly ITeamsRepository _teamsRepository;

        public TeamsService(ITeamsRepository teamsRepository)
        {
            _teamsRepository = teamsRepository;
        }
        public async Task<List<MyTeamDto>> GetUserTeamsAsync(int userId)
        {
            return await _teamsRepository.GetUserTeamsAsync(userId);
        }
    }
}
