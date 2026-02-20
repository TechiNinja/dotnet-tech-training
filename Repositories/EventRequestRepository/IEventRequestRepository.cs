using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;


namespace SportsManagementApp.Repositories;
public interface IEventRequestRepository
{
    Task AddEventRequest(EventRequest request);
    Task<EventRequestResponseDto?> GetEventRequestById(int id);
    Task<IEnumerable<EventRequestResponseDto>> Search(int? id, RequestStatus? status);
    Task<EventRequest?> GetEventRequestEntityById(int id);
    Task UpdateEventRequest(EventRequest request);
    Task<bool> EventRequestExist(CreateEventRequestDto dto);
}