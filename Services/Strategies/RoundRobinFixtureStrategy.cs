using SportsManagementApp.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public sealed class RoundRobinFixtureStrategy : IFixtureStrategy
    {
        public TournamentType TournamentType => TournamentType.RoundRobin;

        public List<Match> Generate(List<int?> sideIds, int categoryId)
        {
            var matches     = new List<Match>();
            var matchNumber = 1;
            var bracketPos  = 1;
            var roundNumber = 1;
            var sides       = sideIds.ToList();

            if (sides.Count % 2 != 0) sides.Add(null);

            int totalRounds = sides.Count - 1;
            int halfSize    = sides.Count / 2;

            for (int round = 0; round < totalRounds; round++)
            {
                for (int i = 0; i < halfSize; i++)
                {
                    var sideA = sides[i];
                    var sideB = sides[sides.Count - 1 - i];
                    if (!sideA.HasValue && !sideB.HasValue) continue;

                    matches.Add(BuildMatch(categoryId, sideA, sideB, roundNumber, matchNumber++, bracketPos++));
                }

                var last = sides[sides.Count - 1];
                sides.RemoveAt(sides.Count - 1);
                sides.Insert(1, last);
                roundNumber++;
            }

            return matches;
        }

        private static Match BuildMatch(
            int categoryId, int? sideA, int? sideB,
            int round, int matchNum, int bracketPos) => new()
        {
            EventCategoryId = categoryId,
            SideAId         = sideA,
            SideBId         = sideB,
            RoundNumber     = round,
            MatchNumber     = matchNum,
            BracketPosition = bracketPos,
            Status          = MatchStatus.Upcoming,
            MatchVenue      = string.Empty,
            MatchDateTime   = default,
            CreatedAt       = DateTime.UtcNow
        };
    }
}
