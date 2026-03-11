using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Predicates
{
    public static class SportPredicateBuilder
    {
        public static Expression<Func<Sport, bool>> Build(SportFilterDto filter)
        {
            return sport => string.IsNullOrEmpty(filter.Name) || EF.Functions.Like(sport.Name, $"%{filter.Name}%");
        }
    }
}