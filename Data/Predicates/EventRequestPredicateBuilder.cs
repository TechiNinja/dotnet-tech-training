using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Data.Predicates
{
    public static class EventRequestPredicateBuilder
    {
        public static Expression<Func<EventRequest, bool>> Build(EventRequestFilterDto filter)
        {
            return request =>
                (!filter.Id.HasValue || request.Id == filter.Id.Value) &&
                (!filter.Status.HasValue || request.Status == filter.Status.Value) &&
                (!filter.AdminId.HasValue || request.AdminId == filter.AdminId.Value);
        }
    }
}