using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IOperationsService
    {
        Task<EventRequestResponseDto> ReviewEventRequestAsync(
            int requestId,
            ReviewEventRequestDto dto,
            int opsUserId);
    }
}