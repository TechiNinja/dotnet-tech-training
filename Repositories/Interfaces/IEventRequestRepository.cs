using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
namespace SportsManagementApp.Repositories.Interfaces;

public interface IEventRequestRepository : IGenericRepository<EventRequest>
{
    Task<EventRequest?> GetEventRequestById(int id); 
    Task<IEnumerable<EventRequest>> Search(int? id, RequestStatus? status);
}