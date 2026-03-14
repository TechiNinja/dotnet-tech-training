using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IOperationsService
    {
        Task<EventRequestResponseDto> DecideAsync(
            int requestId,
            DecideEventRequestDto dto,
            int opsUserId,
            RequestStatus status);
    }
}