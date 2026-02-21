using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context) => _context = context;

        public async Task<Event?> GetByIdAsync(int id, bool includeCategories = false)
        {
            var query = _context.Events
                .Include(e => e.Sport)
                .Include(e => e.Organizer)
                .AsQueryable();

            if (includeCategories)
                query = query.Include(e => e.Categories);

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetAllAsync() =>
            await _context.Events
                .Include(e => e.Sport)
                .Include(e => e.Organizer)
                .Include(e => e.Categories)
                .AsNoTracking()
                .ToListAsync();

        
        public async Task<bool> ExistsByNameSportDateAsync(string name, int sportId, DateOnly startDate) =>
            await _context.Events.AnyAsync(e =>
                e.Name == name &&
                e.SportId == sportId &&
                e.StartDate == startDate);

        public async Task AddAsync(Event eventEntity) =>
            await _context.Events.AddAsync(eventEntity);

        public void Update(Event eventEntity) =>
            _context.Events.Update(eventEntity);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }

    public class EventRequestRepository : IEventRequestRepository
    {
        private readonly AppDbContext _context;

        public EventRequestRepository(AppDbContext context) => _context = context;

        public async Task<EventRequest?> GetByIdAsync(int id) =>
            await _context.EventRequests
                .Include(r => r.Sport)
                .FirstOrDefaultAsync(r => r.Id == id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User?> GetByIdWithRoleAsync(int userId) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
    }
}