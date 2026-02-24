using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class SportService: ISportService
    {
        private readonly ISportRepository _sportRepository;

        public SportService(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        public async Task<Sport> CreateSportAsync(CreateSportDto createSport)
        {
            if (string.IsNullOrWhiteSpace(createSport.Name))
            {
                throw new Exception("Sport Name is required");
            }

            var exists = await _sportRepository.SportExistsAsync(createSport.Name.Trim());

            if (exists)
            {
                throw new Exception("Sport already exists");
            }

            return await _sportRepository.CreateSportAsync(createSport.Name.Trim());
        }

        public async Task<IEnumerable<Sport>> GetSportsAsync()
        {
            return await _sportRepository.GetSportsAsync();
        }

        public async Task<Sport> UpdateSportAsync(int id, UpdateSportDto updateSport)
        {
            if (string.IsNullOrEmpty(updateSport.Name))
            {
                throw new Exception("Sport name is required");
            }

            var sport = await _sportRepository.GetSportByIdAsync(id);

            if (sport == null)
            {
                throw new Exception("Sport not found");
            }

            var exists = await _sportRepository.SportExistsAsync(updateSport.Name.Trim());

            if (exists && sport.Name.ToLower() != updateSport.Name.ToLower())
            {
                throw new Exception("Sport with this name already exists");
            }

            sport.Name = updateSport.Name.Trim();
            sport.UpdatedAt = DateTime.UtcNow;

            await _sportRepository.UpdateSportAsync(sport);
            return sport;
        }
    }
}
