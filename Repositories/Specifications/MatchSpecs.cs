using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Specifications
{
    public class MatchByIdSpec : ISpecification<Match>
    {
        private readonly int _matchId;
        public MatchByIdSpec(int matchId) => _matchId = matchId;
        public Expression<Func<Match, bool>> ToExpression() => m => m.Id == _matchId;
    }

    public class MatchByRoundAndBracketSpec : ISpecification<Match>
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
}