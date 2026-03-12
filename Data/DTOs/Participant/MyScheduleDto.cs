namespace SportsManagementApp.Data.DTOs.Participant
{
    public class MyScheduleDto
    {
        public int MatchId { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string SideA { get; set; } = string.Empty;
        public string SideB { get; set; } = string.Empty;
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public string EventName { get; set; } = string.Empty;
    }
}
