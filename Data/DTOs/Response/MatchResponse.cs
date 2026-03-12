namespace SportsManagementApp.DTOs.Response
{
    public class MatchSetResponse
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SetNumber { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MatchResultResponse
    {
        public int Id { get; set; }
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SetUpdateResponse
    {
        public MatchSetResponse Set { get; set; } = null!;
        public MatchResultResponse? Result { get; set; }
    }
}