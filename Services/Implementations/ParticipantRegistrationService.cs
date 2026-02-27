using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class ParticipantRegistrationService: IParticipantRegistrationService
    {
        private readonly IParticipantRegistrationRepository _registrationRepository;

        public ParticipantRegistrationService(IParticipantRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<ParticipantRegistrationResponseDto> RegisterParticipantAsync(ParticipantRegistrationRequestDto request)
        {
            bool exists = await _registrationRepository.IsUserRegisteredInCategoryAsync(request.UserId, request.EventCategoryId);

            if (exists)
            {
                throw new Exception("Participant already registered in this category");
            }

            var registration = new ParticipantRegistration
            {
                UserId = request.UserId,
                EventCategoryId = request.EventCategoryId,
                CreatedAt = DateTime.UtcNow,
            };

            await _registrationRepository.AddAsync(registration);

            var saved = await _registrationRepository.GetParticipantsByIdWithUserAsync(registration.Id);
            if (saved == null)
            {
                throw new Exception("Registration not saved correctly");
            }

            return new ParticipantRegistrationResponseDto
            {
                Id = saved!.Id,
                UserId = saved.UserId,
                Name = saved.User!.FullName,
                EventCategoryId = saved.EventCategoryId,
                RegisteredAt = saved.CreatedAt
            };
        }

        public async Task<List<ParticipantRegistrationResponseDto>> GetRegistrationsByCategoryAsync(int categoryId)
        {
            var registrations = await _registrationRepository.GetParticipantsByCategoryAsync(categoryId);

            return registrations.Select(registration => new ParticipantRegistrationResponseDto
            {
                Id = registration.Id,
                UserId = registration.UserId,
                Name = registration.User!.FullName,
                EventCategoryId = registration.EventCategoryId,
                RegisteredAt = registration.CreatedAt
            }).ToList();
        }
    }
}
