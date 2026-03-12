namespace SportsManagementApp.DTOs.Response
{
    public class StandingsResponse
    {
        public int    CategoryId { get; set; }
        public string TournamentType { get; set; } = string.Empty;
        public List<StandingsEntryResponse> Entries { get; set; } = new();
    }
}
