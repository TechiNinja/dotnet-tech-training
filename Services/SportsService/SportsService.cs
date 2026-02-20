using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Helper;
using SportsManagementApp.Repositories.SportsRepository;

namespace SportsManagementApp.Services
{
    public class SportService : ISportService
    {
        private readonly ISportRepository _sportRepository;

        public SportService(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        public async Task<Sport> CreateSport(CreateSportDto dto)
        {
            var name = dto.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception(StringConstant.sportsNameRequired);
            }


            var existing = await _sportRepository.GetSportByName(name);
            if (existing != null)
            {
                throw new Exception(StringConstant.sportsExist);
            }

            var sport = new Sport
            {
                Name = name,
                CreatedAt = DateTime.UtcNow
            };

            return await _sportRepository.AddSport(sport);
        }

        public async Task<IEnumerable<Sport>> SearchSports(int? id, string? name)
        {
            name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

            return await _sportRepository.SearchSports(id, name);
        }


    }
}
