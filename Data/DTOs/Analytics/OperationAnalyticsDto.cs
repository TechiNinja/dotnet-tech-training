namespace SportsManagementApp.Data.DTOs.Analytics
{
    public class OperationAnalyticsDto
    {
        public int PendingRequests { get; set; }
        public int TotalRequests { get; set; }
        public int TotalEvents { get; set; }
        public int MatchesToday { get; set; }
    }
}