using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public sealed class KnockoutFixtureStrategy : IFixtureStrategy
    {
        public TournamentType TournamentType => TournamentType.Knockout;

        public List<Match> Generate(List<int?> sideIds, int categoryId)
        {
            var matches     = new List<Match>();
            var matchNumber = 1;
            var bracketPos  = 1;
            var roundNumber = 1;

            var current = sideIds.ToList();
            int totalSlots = NextPowerOfTwo(current.Count);
            while (current.Count < totalSlots) current.Add(null);

            while (current.Count > 1)
            {
                var next = new List<int?>();
                for (int i = 0; i < current.Count; i += 2)
                {
                    var sideA = current[i];
                    var sideB = i + 1 < current.Count ? current[i + 1] : null;

                    matches.Add(BuildMatch(categoryId, sideA, sideB, roundNumber, matchNumber++, bracketPos++));

                    next.Add(sideA.HasValue && !sideB.HasValue ? sideA
                           : !sideA.HasValue && sideB.HasValue ? sideB
                           : null);
                }
                current = next;
                roundNumber++;
            }

            return matches;
        }

        private static int NextPowerOfTwo(int n)
        {
            int p = 1;
            while (p < n) p *= 2;
            return p;
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
