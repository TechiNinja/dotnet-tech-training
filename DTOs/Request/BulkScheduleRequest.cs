using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class BulkScheduleRequest
    {
        [Required]
        public List<MatchScheduleItem> Schedules { get; set; } = new();
    }
}
