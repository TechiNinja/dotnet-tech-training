using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Implementations;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class TeamsService: ITeamsService
    {
        private readonly ITeamsRepository _teamsRepository;
        private readonly IParticipantRegistrationRepository _participantRepository;

        public TeamsService(ITeamsRepository teamsRepository, IParticipantRegistrationRepository participantRegistrationRepository)
        {
            _teamsRepository = teamsRepository;
            _participantRepository = participantRegistrationRepository;
        }
        public async Task<List<MyTeamDto>> GetUserTeamsAsync(int userId)
        {
            return await _teamsRepository.GetUserTeamsAsync(userId);
        }

        public async Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request)
        {
            var registration = await _participantRepository.GetParticipantsByCategoryAsync(request.EventCategoryId);

            if (registration.Count < 2)
            {
                throw new Exception("Not enough participants to form teams");
            }

            if (registration.Count % 2 != 0)
            {
                registration.RemoveAt(registration.Count - 1);
            }

            //var random = new Random();
            //registration = registration.OrderBy(r => random.Next()).ToList();

            var random = new Random();
            for (int i = registration.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (registration[i], registration[j]) = (registration[j], registration[i]);
            }

            int teamNumber = 1;
            var result = new List<TeamResponseDto>();

            for (int index = 0; index < registration.Count; index += 2)
            {
                var team = new Team
                {
                    Name = $"Team {teamNumber}",
                    EventCategoryId = request.EventCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    Members =
                    [
                        new TeamMember
                        {
                            UserId = registration[index].UserId
                        },
                        new TeamMember
                        {
                            UserId = registration[index + 1].UserId
                        }
                    ]
                };

                await _teamsRepository.AddAsync(team);

                result.Add(new TeamResponseDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Members =
                    [
                        registration[index].User!.FullName,
                        registration[index + 1].User!.FullName
                    ]
                });

                teamNumber++;
            }
            return result;
        }

        public async Task<List<TeamResponseDto>> GetTeamsByCategoryAsync(int categoryId)
        {
            var teams = await _teamsRepository.GetTeamsByCategoryAsync(categoryId);

            return [.. teams.Select(team => new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Members = [.. team.Members.Select(member => member.User!.FullName)]
            })];
        }
    }
}
