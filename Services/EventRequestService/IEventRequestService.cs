using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services;
public interface IEventRequestService
{
    Task<EventRequest> RaiseEventRequest(CreateEventRequestDto dto, int adminId);
    Task<IEnumerable<EventRequestResponseDto>> GetAllEventRequest();
    Task<EventRequestResponseDto?> GetEventRequestById(int id);
    Task<IEnumerable<EventRequestResponseDto>> GetEventRequestByStatus(RequestStatus status);
    Task<EventRequest> WithdrawlEventRequest(int id);
    Task<EventRequest> EditEventRequest(int id ,EditEventRequestDto dto);
}