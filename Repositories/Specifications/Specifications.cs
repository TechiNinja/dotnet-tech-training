using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Specifications
{
    public sealed class EventByIdSpec : ISpecification<Event>
    {
        private readonly int _id;
        public EventByIdSpec(int id) => _id = id;
        public Expression<Func<Event, bool>> ToExpression() => e => e.Id == _id;
    }

    public sealed class EventByNameContainsSpec : ISpecification<Event>
    {
        private readonly string _name;
        public EventByNameContainsSpec(string name) => _name = name;
        public Expression<Func<Event, bool>> ToExpression() => e => e.Name.Contains(_name);
    }

    public sealed class EventByRequestIdSpec : ISpecification<Event>
    {
        private readonly int _requestId;
        public EventByRequestIdSpec(int requestId) => _requestId = requestId;
        public Expression<Func<Event, bool>> ToExpression() => e => e.EventRequestId == _requestId;
    }

    public sealed class EventBySportSpec : ISpecification<Event>
    {
        private readonly int _sportId;
        public EventBySportSpec(int sportId) => _sportId = sportId;
        public Expression<Func<Event, bool>> ToExpression() => e => e.SportId == _sportId;
    }

    public sealed class EventByStatusSpec : ISpecification<Event>
    {
        private readonly EventStatus _status;
        public EventByStatusSpec(EventStatus status) => _status = status;
        public Expression<Func<Event, bool>> ToExpression() => e => e.Status == _status;
    }

    public sealed class MatchByCategorySpec : ISpecification<Match>
    {
        private readonly int _categoryId;
        public MatchByCategorySpec(int categoryId) => _categoryId = categoryId;
        public Expression<Func<Match, bool>> ToExpression() => m => m.EventCategoryId == _categoryId;
    }

    public sealed class MatchByStatusSpec : ISpecification<Match>
    {
        private readonly MatchStatus _status;
        public MatchByStatusSpec(MatchStatus status) => _status = status;
        public Expression<Func<Match, bool>> ToExpression() => m => m.Status == _status;
    }

    public sealed class MatchNotCompletedSpec : ISpecification<Match>
    {
        public Expression<Func<Match, bool>> ToExpression() => m => m.Status != MatchStatus.Completed;
    }

    public sealed class MatchFromDateTimeSpec : ISpecification<Match>
    {
        private readonly DateTime _from;
        public MatchFromDateTimeSpec(DateTime from) => _from = from;
        public Expression<Func<Match, bool>> ToExpression() => m => m.MatchDateTime >= _from;
    }

    public sealed class MatchExcludeIdSpec : ISpecification<Match>
    {
        private readonly int _excludeId;
        public MatchExcludeIdSpec(int excludeId) => _excludeId = excludeId;
        public Expression<Func<Match, bool>> ToExpression() => m => m.Id != _excludeId;
    }

    public sealed class MatchWithinTimeWindowSpec : ISpecification<Match>
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        public MatchWithinTimeWindowSpec(DateTime center, TimeSpan buffer)
        {
            _from = center.Subtract(buffer);
            _to   = center.Add(buffer);
        }

        public Expression<Func<Match, bool>> ToExpression() =>
            m => m.MatchDateTime >= _from && m.MatchDateTime <= _to;
    }

    public sealed class MatchByIdSpec : ISpecification<Match>
    {
        private readonly int _matchId;
        public MatchByIdSpec(int matchId) => _matchId = matchId;
        public Expression<Func<Match, bool>> ToExpression() => m => m.Id == _matchId;
    }

    public sealed class MatchByRoundAndBracketSpec : ISpecification<Match>
    {
        private readonly int _catId;
        private readonly int _round;
        private readonly int _bracketPos;

        public MatchByRoundAndBracketSpec(int catId, int round, int bracketPos)
        {
            _catId      = catId;
            _round      = round;
            _bracketPos = bracketPos;
        }

        public Expression<Func<Match, bool>> ToExpression() =>
            m => m.EventCategoryId == _catId &&
                 m.RoundNumber     == _round &&
                 m.BracketPosition == _bracketPos;
    }

    public static class SpecificationExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, ISpecification<T> spec)
            => query.Where(spec.ToExpression());

        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
            => new AndSpecification<T>(left, right);

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
            => new OrSpecification<T>(left, right);
    }

    internal sealed class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left  = left;
            _right = right;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            var param = Expression.Parameter(typeof(T));
            var body  = Expression.AndAlso(
                Expression.Invoke(_left.ToExpression(),  param),
                Expression.Invoke(_right.ToExpression(), param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    internal sealed class OrSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left  = left;
            _right = right;
        }

        public Expression<Func<T, bool>> ToExpression()
        {
            var param = Expression.Parameter(typeof(T));
            var body  = Expression.OrElse(
                Expression.Invoke(_left.ToExpression(),  param),
                Expression.Invoke(_right.ToExpression(), param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}