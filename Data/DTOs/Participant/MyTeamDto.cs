namespace SportsManagementApp.Data.DTOs.Participant
{
    public class MyTeamDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
    }
}
