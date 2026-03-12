using AutoMapper;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class ParticipantRegistrationService: IParticipantRegistrationService
    {
        private readonly IParticipantRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;

        public ParticipantRegistrationService(IParticipantRegistrationRepository registrationRepository, IMapper mapper)
        {
            _registrationRepository = registrationRepository;
            _mapper = mapper;
        }

        public async Task<ParticipantRegistrationResponseDto> RegisterParticipantAsync(ParticipantRegistrationRequestDto request)
        {
            bool exists = await _registrationRepository.IsUserRegisteredInCategoryAsync(request.UserId, request.EventCategoryId);

            if (exists)
            {
                throw new ConflictException("Participant already registered in this category");
            }

            var registration = _mapper.Map<ParticipantRegistration>(request);

            await _registrationRepository.AddAsync(registration);

            var saved = await _registrationRepository.GetParticipantsByIdWithUserAsync(registration.Id);
            if (saved == null)
            {
                throw new BadRequestException("Registration not saved correctly");
            }

            return _mapper.Map<ParticipantRegistrationResponseDto>(saved);
        }

        public async Task<List<ParticipantRegistrationResponseDto>> GetRegistrationsByCategoryAsync(int categoryId)
        {
            var registrations = await _registrationRepository.GetParticipantsByCategoryAsync(categoryId);

            return _mapper.Map<List<ParticipantRegistrationResponseDto>>(registrations);
        }
    }
}
