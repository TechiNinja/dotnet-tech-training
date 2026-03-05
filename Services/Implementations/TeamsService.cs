using AutoMapper;
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
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository teamsRepository, IParticipantRegistrationRepository participantRegistrationRepository, IMapper mapper)
        {
            _teamsRepository = teamsRepository;
            _participantRepository = participantRegistrationRepository;
            _mapper = mapper;
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
            for (int index = registration.Count - 1; index > 0; index--)
            {
                int iterator = random.Next(index + 1);
                (registration[index], registration[iterator]) = (registration[iterator], registration[index]);
            }

            int teamNumber = 1;
            var result = new List<TeamResponseDto>();

            for (int index = 0; index < registration.Count; index += 2)
            {
                var team = _mapper.Map<Team>(request);
                team.Name = $"Team {teamNumber}";
                team.CreatedAt = DateTime.UtcNow;

                team.Members = [
                    new TeamMember { UserId = registration[index].UserId },
                    new TeamMember { UserId = registration[index + 1].UserId }
                ];

                await _teamsRepository.AddAsync(team);

                result.Add(_mapper.Map<TeamResponseDto>(team));
                teamNumber++;
            }
            return result;
        }

        public async Task<List<TeamResponseDto>> GetTeamsByCategoryAsync(int categoryId)
        {
            var teams = await _teamsRepository.GetTeamsByCategoryAsync(categoryId);

            return _mapper.Map<List<TeamResponseDto>>(teams);
        }
    }
}
