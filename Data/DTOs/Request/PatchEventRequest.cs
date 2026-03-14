using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class PatchEventRequest
    {
        [Required]
        public string Action { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = AppConstants.EventNameTooLong)]
        public string? Name { get; set; }

        [MaxLength(1000, ErrorMessage = AppConstants.DescriptionTooLong)]
        public string? Description { get; set; }

        [Range(2, 10000, ErrorMessage = AppConstants.MaxParticipantsRange)]
        public int? MaxParticipantsCount { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }
    }
}