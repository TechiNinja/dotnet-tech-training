using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SportRepository: ISportRepository
    {
        private readonly AppDbContext _context;

        public SportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SportExistsAsync(string name)
        {
            return await _context.Sports
                .AnyAsync(sport => sport.Name.ToLower() == name.ToLower());
        }

        public async Task<Sport> CreateSportAsync(string name)
        {
            var sport = new Sport
            {
                Name = name.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            return sport;
        }

        public async Task<IEnumerable<Sport>> GetSportsAsync()
        {
            return await _context.Sports.ToListAsync();
        }
    }
}
