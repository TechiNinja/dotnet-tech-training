using SportsManagementApp.Enums;
using SportsManagementApp.StringConstants;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.DTOs.Request
{
    public class CreateEventRequest
    {
        [Required(ErrorMessage = AppConstants.EventRequestIdRequired)]
        public int EventRequestId { get; set; }

        [Required(ErrorMessage = AppConstants.EventVenueRequired)]
        [MaxLength(200, ErrorMessage = AppConstants.EventVenueTooLong)]
        public string EventVenue { get; set; } = string.Empty;

        [Required(ErrorMessage = AppConstants.RegistrationDeadlineRequired)]
        public DateOnly RegistrationDeadline { get; set; }

        [MaxLength(1000, ErrorMessage = AppConstants.DescriptionTooLong)]
        public string Description { get; set; } = string.Empty;
    }

    public class AssignOrganizerRequest
    {
        [Required(ErrorMessage = AppConstants.RequiredValueMissing)]
        public int OrganizerId { get; set; }
    }

    public class EventConfigurationRequest
    {
        public EventRulesRequest? Rules { get; set; }
        public List<EventCategoryRequest> Categories { get; set; } = new();
        public TeamLimitsRequest? TeamLimits { get; set; }
    }

    public class EventRulesRequest
    {
        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public DateOnly RegistrationDeadline { get; set; }

        [Required, MaxLength(200)]
        public string EventVenue { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }

    public class EventCategoryRequest
    {
        [Required]
        public MatchFormat Format { get; set; }

        [Required]
        public GenderType Gender { get; set; }

        [Required]
        public TournamentType TournamentType { get; set; }
    }

    public class TeamLimitsRequest
    {
        [Range(2, 256, ErrorMessage = AppConstants.MaxTeamsRangeError)]
        public int MaxTeamsPerCategory { get; set; } = 8;

        [Range(1, 50, ErrorMessage = AppConstants.MaxMembersRangeError)]
        public int MaxMembersPerTeam { get; set; } = 10;
    }
}