using AutoMapper;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings
{
    public class ParticipantRegistrationProfile : Profile
    {
        public ParticipantRegistrationProfile()
        {
            CreateMap<ParticipantRegistration, ParticipantRegistrationResponseDto>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.User!.FullName));

            CreateMap<ParticipantRegistrationRequestDto, ParticipantRegistration>()
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.MapFrom(_ => DateTime.UtcNow));

        }
    }
}
