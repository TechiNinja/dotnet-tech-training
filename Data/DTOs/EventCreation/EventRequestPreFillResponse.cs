namespace SportsManagementApp.DTOs.EventCreation
{
    public class EventRequestPreFillResponseDto
    {
        public int Id { get; set; }
        public string SportName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string RequestedVenue { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int MaxParticipantsCount { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateOnly? RegistrationDeadline { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsEventAlreadyCreated { get; set; }
    }
}