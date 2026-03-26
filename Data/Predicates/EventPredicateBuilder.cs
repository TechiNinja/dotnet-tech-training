using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Exceptions;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Enums;
using System.Linq.Expressions;

public static class EventPredicateBuilder
{
    public static Expression<Func<Event, bool>> Build(EventFilterDto filter)
    {
        EventStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (!Enum.TryParse<EventStatus>(filter.Status, true, out var status))
                throw new BadRequestException(
                    string.Format(StringConstant.InvalidEventStatus, filter.Status));
            parsedStatus = status;
        }

        return e =>
            (parsedStatus == null || e.Status == parsedStatus.Value) &&
            (string.IsNullOrWhiteSpace(filter.Name) || e.Name.Contains(filter.Name)) &&
            (!filter.SportId.HasValue || e.SportId == filter.SportId.Value);
    }
}