    using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Filters
{
    public class EventRequestFilterDto
    {
        public int? Id { get; set; }
        public RequestStatus? Status { get; set; }
        public int? AdminId { get; set; }
    }
}