namespace SportsManagementApp.DTOs.Fixture
{
    public class FixtureResponseDto
    {
        public int Id { get; set; }
        public int RoundNumber { get; set; }
        public int MatchNumber { get; set; }
        public int BracketPosition { get; set; }
        public int? SideAId { get; set; }
        public string SideAName { get; set; } = string.Empty;
        public int? SideBId { get; set; }
        public string SideBName { get; set; } = string.Empty;
        public DateTime? MatchDateTime { get; set; }
        public string MatchVenue { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsBye { get; set; }
        public int TotalSets { get; set; }
        public List<MatchSetResponseDto> Sets { get; set; } = new();
        public MatchResultResponseDto? Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}