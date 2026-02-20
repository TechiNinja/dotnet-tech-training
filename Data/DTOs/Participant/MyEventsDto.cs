namespace SportsManagementApp.Data.DTOs.Participant
{
    public class MyEventsDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
