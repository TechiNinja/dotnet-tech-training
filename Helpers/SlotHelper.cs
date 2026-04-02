using SportsManagementApp.Exceptions;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Helpers
{
    public static class SlotHelper
    {
        public static DateTime GetNextSlot(DateTime current, DateOnly endDate)
        {
            var next = current.AddMinutes(StringConstant.SlotMinutes);

            if (TimeOnly.FromDateTime(next) >= StringConstant.DayEnd)
                next = next.Date.AddDays(1).Add(StringConstant.DayStart.ToTimeSpan());

            if (DateOnly.FromDateTime(next) > endDate)
                throw new BadRequestException(StringConstant.NotEnoughDaysToSchedule);

            return next;
        }
    }
}