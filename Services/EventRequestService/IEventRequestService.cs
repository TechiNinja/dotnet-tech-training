using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services;
public interface IEventRequestService
{
    Task<EventRequestResponseDto> RaiseEventRequest(CreateEventRequestDto dto, int adminId);
    Task<IEnumerable<EventRequestResponseDto>> SearchEventRequests(int? id, RequestStatus? status);
    Task<EventRequestResponseDto> WithdrawlEventRequest(int id);
    Task<EventRequestResponseDto> EditEventRequest(int id ,EditEventRequestDto dto);
}