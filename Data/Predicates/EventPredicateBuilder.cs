using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Predicates
{
    public static class EventPredicateBuilder
    {
        public static Expression<Func<Event, bool>> Build(EventFilterDto filter)
        {
            Enum.TryParse<EventStatus>(filter.Status, true, out var parsedStatus);

            return e =>
                (string.IsNullOrWhiteSpace(filter.Status) || e.Status == parsedStatus) &&
                (string.IsNullOrWhiteSpace(filter.Name)   || e.Name.Contains(filter.Name)) &&
                (!filter.SportId.HasValue                 || e.SportId == filter.SportId.Value);
        }
    }
}