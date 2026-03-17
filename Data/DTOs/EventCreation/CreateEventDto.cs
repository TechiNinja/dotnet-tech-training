using System.ComponentModel.DataAnnotations;
using SportsManagementApp.Constants;

namespace SportsManagementApp.DTOs.EventCreation
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = StringConstant.EventRequestIdRequired)]
        public int EventRequestId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = StringConstant.EventNameTooLong)]
        public string? Name { get; set; }

        [Required(ErrorMessage = StringConstant.RegistrationDeadlineRequired)]
        public DateOnly RegistrationDeadline { get; set; }

        [MaxLength(1000, ErrorMessage = StringConstant.DescriptionTooLong)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = StringConstant.MaxParticipantsRequired)]
        [Range(2, 10000, ErrorMessage = StringConstant.MaxParticipantsRange)]
        public int MaxParticipantsCount { get; set; }
    }
}