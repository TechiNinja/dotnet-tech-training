using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class CreateEventRequest
    {
        [Required(ErrorMessage = AppConstants.EventRequestIdRequired)]
        public int EventRequestId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = AppConstants.EventNameTooLong)]
        public string? Name { get; set; }

        [Required(ErrorMessage = AppConstants.RegistrationDeadlineRequired)]
        public DateOnly RegistrationDeadline { get; set; }

        [MaxLength(1000, ErrorMessage = AppConstants.DescriptionTooLong)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = AppConstants.MaxParticipantsRequired)]
        [Range(2, 10000, ErrorMessage = AppConstants.MaxParticipantsRange)]
        public int MaxParticipantsCount { get; set; }
    }
}
