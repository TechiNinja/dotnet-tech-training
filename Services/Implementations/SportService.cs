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

            var exists = await _sportRepository.SportExistsAsync(createSport.Name);

            if (exists)
            {
                throw new Exception("Sport already exists");
            }

            return await _sportRepository.CreateSportAsync(createSport.Name);
        }

        public async Task<IEnumerable<Sport>> GetSportsAsync()
        {
            return await _sportRepository.GetSportsAsync();
        }
    }
}
