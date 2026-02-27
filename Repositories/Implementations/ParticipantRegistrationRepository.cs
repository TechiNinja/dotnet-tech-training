using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class ParticipantRegistrationRepository: GenericRepository<ParticipantRegistration>, IParticipantRegistrationRepository
    {
        public ParticipantRegistrationRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsUserRegisteredInCategoryAsync(int userId, int categoryId)
        {
            return await _dbSet.AnyAsync(registration => registration.UserId == userId && registration.EventCategoryId == categoryId);
        }

        public async Task<List<ParticipantRegistration>> GetParticipantsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Include(registration => registration.User)
                .Where(registration => registration.EventCategoryId == categoryId)
                .OrderBy(registration => registration.CreatedAt)
                .ToListAsync();
        }

        public async Task<ParticipantRegistration?> GetParticipantsByIdWithUserAsync(int id)
        {
            return await _dbSet
                .Include(registration => registration.User)
                .FirstOrDefaultAsync(registration => registration.Id == id);
        }
    }
}
