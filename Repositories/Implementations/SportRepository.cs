using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SportRepository : GenericRepository<Sport>, ISportRepository
    {
        public SportRepository(AppDbContext context) : base(context) { }

        public async Task<bool> SportExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(sport => sport.Name == name);
        }
    }
}
