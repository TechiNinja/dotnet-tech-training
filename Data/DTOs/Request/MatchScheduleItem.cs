using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class MatchScheduleItem
    {
        [Required]
        public int MatchId { get; set; }

        [Required(ErrorMessage = AppConstants.MatchDateTimeRequired)]
        public DateTime MatchDateTime { get; set; }
        [Range(1, 10)]
        public int TotalSets { get; set; } = 1;
    }
}
