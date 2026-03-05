namespace SportsManagementApp.DTOs.Response
{
    public class TeamResponse
    {
        public int    Id   { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TeamMemberResponse> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
