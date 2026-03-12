using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Interfaces;
public interface IEventRequestService
{
    Task<EventRequestResponseDto> RaiseEventRequest(CreateEventRequestDto dto, int adminId);
    Task<IEnumerable<EventRequestResponseDto>> SearchEventRequests(int? id, RequestStatus? status);
    Task<EventRequestResponseDto> WithdrawEventRequest(int id);
    Task<EventRequestResponseDto> EditEventRequest(int id ,EditEventRequestDto dto);
}