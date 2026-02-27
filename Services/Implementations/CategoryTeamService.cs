using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class CategoryTeamService: ICategoryTeamService
    {
        private readonly IParticipantRegistrationRepository _registrationRepository;
        private readonly ICategoryTeamRepository _teamRepository;

        public CategoryTeamService(ICategoryTeamRepository teamsRepository, IParticipantRegistrationRepository registrationRepository)
        {
            _teamRepository = teamsRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request)
        {
            var registration = await _registrationRepository.GetParticipantsByCategoryAsync(request.EventCategoryId);

            if (registration.Count < 2)
            {
                throw new Exception("Not enough participants to form teams");
            }

            if (registration.Count % 2 != 0)
            {
                registration.RemoveAt(registration.Count - 1);
            }

            var random = new Random();
            registration = registration.OrderBy(r => random.Next()).ToList();

            int teamNumber = 1;
            var result = new List<TeamResponseDto>();

            for (int index = 0; index < registration.Count; index += 2)
            {
                var team = new Team
                {
                    Name = $"Team {teamNumber}",
                    EventCategoryId = request.EventCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    Members = new List<TeamMember>
                    {
                        new TeamMember
                        {
                            UserId = registration[index].UserId
                        },
                        new TeamMember
                        {
                            UserId = registration[index + 1].UserId
                        }
                    }
                };

                await _teamRepository.AddAsync(team);

                result.Add(new TeamResponseDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Members = new List<string>
                    {
                        registration[index].User!.FullName,
                        registration[index + 1].User!.FullName
                    }
                });

                teamNumber++;
            }
            return result;
        }

        public async Task<List<TeamResponseDto>> GetTeamsByCategoryAsync(int categoryId)
        {
            var teams = await _teamRepository.GetTeamsByCategoryAsync(categoryId);

            return teams.Select(team => new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Members = team.Members
                    .Select(member => member.User!.FullName)
                    .ToList()
            }).ToList();
        }
    }
}
