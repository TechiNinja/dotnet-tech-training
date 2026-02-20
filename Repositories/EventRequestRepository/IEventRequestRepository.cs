using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;


namespace SportsManagementApp.Repositories;
public interface IEventRequestRepository
{
    Task AddEventRequest(EventRequest request);
    Task<List<EventRequestResponseDto>> GetAllEventRequest();
    Task<EventRequestResponseDto?> GetEventRequestById(int id);
    Task<List<EventRequestResponseDto>> GetEventRequestByStatus(RequestStatus status);
    Task<EventRequest?> GetEventRequestEntityById(int id);
    Task<List<EventRequest>> GetEventRequestEntityByStatus(RequestStatus status);
    Task<EventRequest> UpdateEventRequest(EventRequest request);
    Task<bool> EventRequestExist(CreateEventRequestDto dto);
}