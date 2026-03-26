using AutoMapper;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
namespace SportsManagementApp.Services.Implementations
{
    public class TeamsService : ITeamsService
    {
        private readonly IGenericRepository<Team> _repository;
        private readonly IParticipantRegistrationRepository _participantRepository;
        private readonly IMapper _mapper;

        public TeamsService(IGenericRepository<Team> repository, IParticipantRegistrationRepository participantRepository, IMapper mapper)
        {
            _repository = repository;
            _participantRepository = participantRepository;
            _mapper = mapper;
        }

        public async Task<List<TeamResponseDto>> GetTeamsAsync(TeamFilterDto filter)
        {
            var predicate = TeamPredicateBuilder.Build(filter);
            return await _repository.GetAllAsync(
                predicate: predicate,
                projection: team => new TeamResponseDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    EventCategoryId = team.EventCategoryId,
                    Members = team.Members.Select(member => member.User != null ? member.User.FullName : "N/A").ToList()
                }
            );
        }

        public async Task<List<TeamResponseDto>> CreateTeamsAsync(CreateTeamRequestDto request)
        {
            var registration = await _participantRepository.GetParticipantsByCategoryAsync(request.EventCategoryId);

            if (registration.Count < 4)
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
            var teamsToAdd = new List<Team>();

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
                teamsToAdd.Add(team);
                teamNumber++;
            }

            await _repository.AddRangeAsync(teamsToAdd);
            await _repository.SaveChangesAsync();

            var filter = new TeamFilterDto { CategoryId = request.EventCategoryId };
            return await GetTeamsAsync(filter);
        }
    }
}
