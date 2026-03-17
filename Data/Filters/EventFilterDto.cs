namespace SportsManagementApp.Data.Filters
{
    public class EventFilterDto
    {
        public int? EventId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? SportId { get; set; }
    }
}