using AutoMapper;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _sportRepository;
        private readonly IMapper _mapper;

        public SportService(ISportRepository sportRepository, IMapper mapper)
        {
            _sportRepository = sportRepository;
            _mapper = mapper;
        }

        public async Task<Sport> CreateSportAsync(CreateSportDto createSport)
        {
            if (string.IsNullOrWhiteSpace(createSport.Name))
            {
                throw new BadRequestException("Sport Name is required");
            }

            var trimmedName = createSport.Name.Trim();

            var exists = await _sportRepository.SportExistsAsync(trimmedName);

            if (exists)
            {
                throw new ConflictException("Sport already exists");
            }

            var sport = _mapper.Map<Sport>(createSport);
            sport.Name = trimmedName;
            sport.CreatedAt = DateTime.UtcNow;

            await _sportRepository.AddAsync(sport);
            await _sportRepository.SaveChangesAsync();

            return sport;
        }

        public async Task<List<SportResponseDto>> GetSportsAsync(SportFilterDto filter)
        {
            var predicate = SportPredicateBuilder.Build(filter);

            return await _sportRepository.GetAllAsync(
                predicate: predicate,
                projection: sport => new SportResponseDto
                {
                    Id = sport.Id,
                    Name = sport.Name
                }
            );
        }

        public async Task<Sport> UpdateSportAsync(int id, UpdateSportDto updateSport)
        {
            if (string.IsNullOrEmpty(updateSport.Name))
            {
                throw new BadRequestException("Sport name is required");
            }

            var sport = await _sportRepository.GetByIdAsync(id);

            if (sport == null)
            {
                throw new NotFoundException("Sport not found");
            }

            var trimmedName = updateSport.Name.Trim();

            var exists = await _sportRepository.SportExistsAsync(trimmedName);

            if (exists && !sport.Name.Equals(updateSport.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException("Sport with this name already exists");
            }

            sport.Name = trimmedName;
            sport.UpdatedAt = DateTime.UtcNow;

            await _sportRepository.UpdateAsync(sport);
            await _sportRepository.SaveChangesAsync();
            return sport;
        }
    }
}
