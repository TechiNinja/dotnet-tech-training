namespace SportsManagementApp.DTOs.EventCreation
{
    public class EventBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int MaxParticipantsCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}