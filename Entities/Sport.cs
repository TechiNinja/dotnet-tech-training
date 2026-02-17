namespace SportsManagementApp.Entities
{
    public class Sport
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
