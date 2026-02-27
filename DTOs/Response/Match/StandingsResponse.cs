namespace SportsManagementApp.DTOs.Response
{
    public class StandingsResponse
    {
        public int CategoryId { get; set; }
        public string TournamentType { get; set; } = string.Empty;
        public List<StandingsEntryResponse> Entries { get; set; } = new();
    }

    public class StandingsEntryResponse
    {
        public int SideId { get; set; }
        public string SideName { get; set; } = string.Empty;
        public int Played { get; set; }
        public int Won { get; set; }
        public int Lost { get; set; }
        public int Drawn { get; set; }
        public int Points { get; set; }
    }
}