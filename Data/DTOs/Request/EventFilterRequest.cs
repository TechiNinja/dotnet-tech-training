namespace SportsManagementApp.DTOs.Request
{
    public class EventFilterRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? SportId { get; set; }
    }
}
