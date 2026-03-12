namespace SportsManagementApp.Data.Filters
{
    public class UserFilterDto
    {
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
