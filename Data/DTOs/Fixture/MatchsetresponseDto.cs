namespace SportsManagementApp.DTOs.Fixture
{
    public class MatchSetResponseDto
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
    public class MatchResultResponseDto
    {
        public int Id { get; set; }
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class SetUpdateResponseDto
    {
        public MatchSetResponseDto Set { get; set; } = null!;
        public MatchResultResponseDto? Result { get; set; }
    }
}