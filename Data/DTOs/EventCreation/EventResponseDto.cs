namespace SportsManagementApp.DTOs.EventCreation
{
    public class EventResponseDto : EventBaseDto
    {
        public string EventVenue { get; set; } = string.Empty;
        public DateOnly RegistrationDeadline { get; set; }
        public string TournamentType { get; set; } = string.Empty;
        public string? OrganizerName { get; set; }
        public List<EventCategoryResponseDto> Categories { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}