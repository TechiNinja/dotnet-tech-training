using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class ScheduleUpdateRequest
    {
        [Required(ErrorMessage = AppConstants.MatchDateTimeRequired)]
        public DateTime MatchDateTime { get; set; }
    }
}