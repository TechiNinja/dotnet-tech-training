using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class EventRequestRepository : GenericRepository<EventRequest>, IEventRequestRepository
    {
        public EventRequestRepository(AppDbContext context) : base(context)
        {
        }
    }
}