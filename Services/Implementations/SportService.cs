using AutoMapper;
using SportsManagementApp.StringConstants;
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
                throw new BadRequestException(StringConstant.SportsNameRequired);
            }

            var trimmedName = createSport.Name.Trim();

            var exists = await _sportRepository.ExistsAsync(
                sport => sport.Name.ToLower() == trimmedName.ToLower()
            );

            if (exists)
            {
                throw new ConflictException(StringConstant.SportsExist);
            }

            var sport = _mapper.Map<Sport>(createSport);

            await _sportRepository.AddAsync(sport);
            await _sportRepository.SaveChangesAsync();

            return sport;
        }

        public async Task<List<SportResponseDto>> GetSportsAsync(SportFilterDto filter)
        {
            var predicate = SportPredicateBuilder.Build(filter);

            var sports = await _sportRepository.GetAllAsync(predicate);

            return _mapper.Map<List<SportResponseDto>>(sports);
        }

        public async Task<Sport> UpdateSportAsync(int id, UpdateSportDto updateSport)
        {
            if (string.IsNullOrWhiteSpace(updateSport.Name))
            {
                throw new BadRequestException(StringConstant.SportsNameRequired);
            }

            var sport = await _sportRepository.GetByIdAsync(id);

            if (sport == null)
            {
                throw new NotFoundException(StringConstant.SportsNotFound);
            }

            var trimmedName = updateSport.Name.Trim();

            var exists = await _sportRepository.ExistsAsync(
                s => s.Name.ToLower() == trimmedName.ToLower() && s.Id != id
            );

            if (exists)
            {
                throw new ConflictException(StringConstant.SportsExist);
            }

            _mapper.Map(updateSport, sport);

            await _sportRepository.UpdateAsync(sport);
            await _sportRepository.SaveChangesAsync();

            return sport;
        }
    }
}