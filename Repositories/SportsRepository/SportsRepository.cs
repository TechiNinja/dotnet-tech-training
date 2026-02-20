using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.SportsRepository
{
    public class SportRepository : ISportRepository
    {
        private readonly AppDbContext _context;

        public SportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sport> AddSport(Sport sport)
        {
            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();
            return sport;
        }

        public async Task<List<Sport>> SearchSports(int? id, string? name)
        {
            var query = _context.Sports.AsNoTracking().AsQueryable();

            if (id.HasValue)
                query = query.Where(s => s.Id == id.Value);

            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim().ToLower();
                query = query.Where(s => s.Name.ToLower() == n);
            }

            return await query.ToListAsync();
        }

        public async Task<Sport?> GetSportByName(string name)
        {
            var n = name.Trim().ToLower();
            return await _context.Sports.FirstOrDefaultAsync(s => s.Name.ToLower() == n);
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Sports.AnyAsync(s => s.Id == id);
        }


    }
}
