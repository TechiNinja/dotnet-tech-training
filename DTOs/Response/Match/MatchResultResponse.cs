namespace SportsManagementApp.DTOs.Response
{
    public class MatchResultResponse
    {
        public int Id { get; set; }
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}