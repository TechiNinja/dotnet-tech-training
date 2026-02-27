using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IParticipantRegistrationService
    {
        Task<ParticipantRegistrationResponseDto> RegisterParticipantAsync(ParticipantRegistrationRequestDto request);
        Task<List<ParticipantRegistrationResponseDto>> GetRegistrationsByCategoryAsync(int categoryId);
    }
}
