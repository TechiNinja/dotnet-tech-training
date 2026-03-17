namespace SportsManagementApp.DTOs.Fixture
{
    public class EventCategoryResponseDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int    EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string TournamentType { get; set; } = string.Empty;
        public int    MaxParticipantsCount { get; set; }
        public DateTime  CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}