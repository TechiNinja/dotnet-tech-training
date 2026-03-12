namespace SportsManagementApp.Data.DTOs.Analytics
{
    public class AdminAnalyticsDto
    {
        public int TotalEvents { get; set; }
        public int ActiveUsers { get; set; }
        public int Teams { get; set; }
        public int MatchesToday { get; set; }
    }
}