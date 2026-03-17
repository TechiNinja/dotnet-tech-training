using AutoMapper;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _teamsRepository;
        private readonly IParticipantRegistrationRepository _participantRepository;
        private readonly IMapper _mapper;

        public TeamsService(ITeamsRepository teamsRepository, IParticipantRegistrationRepository participantRepository, IMapper mapper)
        {
            _teamsRepository = teamsRepository;
            _participantRepository = participantRepository;
            _mapper = mapper;
        }

        public async Task<List<TeamResponseDto>> GetTeamsAsync(TeamFilterDto filter)
        {
            return await _teamsRepository.GetTeamsByFilterAsync(
                TeamPredicateBuilder.Build(filter),
                TeamProjectionBuilder.Build()
            );
        }

        public async Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request)
        {
            var registration = await _participantRepository.GetParticipantsByCategoryAsync(request.EventCategoryId);

            if (registration.Count < 2)
                throw new BadRequestException("Not enough participants to form teams");

            if (registration.Count % 2 != 0)
                registration.RemoveAt(registration.Count - 1);

            var random = new Random();
            for (int index = registration.Count - 1; index > 0; index--)
            {
                int swap = random.Next(index + 1);
                (registration[index], registration[swap]) = (registration[swap], registration[index]);
            }

            int teamNumber = 1;
            var result = new List<TeamResponseDto>();

            for (int index = 0; index < registration.Count; index += 2)
            {
                var team = _mapper.Map<Team>(request);
                team.Name = $"Team {teamNumber}";
                team.CreatedAt = DateTime.UtcNow;
                team.Members = new List<TeamMember>
                {
                    new() { UserId = registration[index].UserId },
                    new() { UserId = registration[index + 1].UserId }
                };

                await _teamsRepository.AddAsync(team);

                result.Add(_mapper.Map<TeamResponseDto>(team));
                teamNumber++;
            }

            await _teamsRepository.SaveChangesAsync();

            return result;
        }
    }
}