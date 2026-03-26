namespace SportsManagementApp.DTOs.Fixture
{
    public class MatchCreateDto
    {
        public int EventCategoryId { get; set; }
        public int? SideAId { get; set; }
        public int? SideBId { get; set; }
        public int RoundNumber { get; set; }
        public int MatchNumber { get; set; }
        public int BracketPosition { get; set; }
    }
}