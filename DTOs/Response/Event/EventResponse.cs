using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.DTOs.Response
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string EventVenue { get; set; } = string.Empty;
        public DateOnly RegistrationDeadline { get; set; }
        public int MaxParticipantsCount { get; set; }
        public string TournamentType { get; set; } = string.Empty;
        public string? OrganizerName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EventCategoryResponse> Categories { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}