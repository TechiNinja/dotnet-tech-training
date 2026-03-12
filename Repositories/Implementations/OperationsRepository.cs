using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations;

public class OperationsRepository : GenericRepository<EventRequest>, IOperationsRepository
{
    public OperationsRepository(AppDbContext context) : base(context) { }

    public async Task<EventRequest?> GetEventRequestByIdAsync(int id)
    {
        return await _context.EventRequests
            .Include(request => request.Sport)
            .Include(request => request.Admin)
            .Include(request => request.OperationsReviewer)
            .FirstOrDefaultAsync(request => request.Id == id);
    }
}