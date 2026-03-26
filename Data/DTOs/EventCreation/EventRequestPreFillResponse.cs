namespace SportsManagementApp.DTOs.EventCreation
{
    public class EventRequestPreFillResponseDto : EventBaseDto
    {
        public string Gender { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string RequestedVenue { get; set; } = string.Empty;
        public DateOnly? RegistrationDeadline { get; set; }
        public bool IsEventAlreadyCreated { get; set; }
    }
}