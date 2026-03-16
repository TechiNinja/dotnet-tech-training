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
            int matchNumber = 1;

            var teams = sideIds
                .Where(id => id.HasValue)
                .OrderBy(_ => Guid.NewGuid())
                .Select(id => id!.Value)
                .ToList();

            if (teams.Count < 2) return matches;

            BuildFromTeams(matches, teams, categoryId, roundNumber: 1, ref matchNumber);
            return matches;
        }

        private static void BuildFromTeams(
            List<Match> matches,
            List<int>   teams,
            int         categoryId,
            int         roundNumber,
            ref int     matchNumber)
        {
            if (teams.Count == 1) return;

            if (teams.Count % 2 == 0)
                BuildEvenRound(matches, teams, categoryId, roundNumber, ref matchNumber);
            else
                BuildOddRound(matches, teams, categoryId, roundNumber, ref matchNumber);
        }

        private static void BuildEvenRound(
            List<Match> matches,
            List<int>   teams,
            int         categoryId,
            int         roundNumber,
            ref int     matchNumber)
        {
            int pairs = teams.Count / 2;

            for (int i = 0; i < pairs; i++)
            {
                matches.Add(BuildMatch(
                    categoryId,
                    sideA:           teams[i * 2],
                    sideB:           teams[i * 2 + 1],
                    roundNumber:     roundNumber,
                    matchNumber:     matchNumber,
                    bracketPosition: i));
                matchNumber++;
            }

            BuildPlaceholderRounds(matches, slotCount: pairs, categoryId, roundNumber + 1, ref matchNumber);
        }

        private static void BuildOddRound(
            List<Match> matches,
            List<int>   teams,
            int         categoryId,
            int         roundNumber,
            ref int     matchNumber)
        {
            int byeTeamId = teams[0];
            var remaining = teams.Skip(1).ToList();

            int pairs = remaining.Count / 2;
            for (int i = 0; i < pairs; i++)
            {
                matches.Add(BuildMatch(
                    categoryId,
                    sideA:           remaining[i * 2],
                    sideB:           remaining[i * 2 + 1],
                    roundNumber:     roundNumber,
                    matchNumber:     matchNumber,
                    bracketPosition: i));
                matchNumber++;
            }

            int currentSlots = pairs;
            int currentRound = roundNumber + 1;

            while (currentSlots > 2)
            {
                if (currentSlots % 2 != 0)
                {
                    BuildPlaceholderOddRounds(matches, currentSlots, categoryId, currentRound, ref matchNumber);
                    return;
                }

                int halfSlots = currentSlots / 2;
                for (int i = 0; i < halfSlots; i++)
                {
                    matches.Add(BuildMatch(
                        categoryId, null, null,
                        currentRound, matchNumber,
                        bracketPosition: i));
                    matchNumber++;
                }
                currentSlots = halfSlots;
                currentRound++;
            }

            EmitSemiFinalBlock(matches, byeTeamId, categoryId, currentRound, ref matchNumber);
        }

        private static void BuildPlaceholderRounds(
            List<Match> matches,
            int         slotCount,
            int         categoryId,
            int         roundNumber,
            ref int     matchNumber)
        {
            if (slotCount <= 1) return;

            if (slotCount % 2 == 0)
            {
                int pairs = slotCount / 2;
                for (int i = 0; i < pairs; i++)
                {
                    matches.Add(BuildMatch(
                        categoryId, null, null,
                        roundNumber, matchNumber,
                        bracketPosition: i));
                    matchNumber++;
                }
                BuildPlaceholderRounds(matches, pairs, categoryId, roundNumber + 1, ref matchNumber);
            }
            else
            {
                BuildPlaceholderOddRounds(matches, slotCount, categoryId, roundNumber, ref matchNumber);
            }
        }

        private static void BuildPlaceholderOddRounds(
            List<Match> matches,
            int         slotCount,
            int         categoryId,
            int         roundNumber,
            ref int     matchNumber)
        {
            int remainingSlots = slotCount - 1;
            int pairs          = remainingSlots / 2;

            for (int i = 0; i < pairs; i++)
            {
                matches.Add(BuildMatch(
                    categoryId, null, null,
                    roundNumber, matchNumber,
                    bracketPosition: i));
                matchNumber++;
            }

            int currentSlots = pairs;
            int currentRound = roundNumber + 1;

            while (currentSlots > 2)
            {
                if (currentSlots % 2 != 0)
                {
                    BuildPlaceholderOddRounds(matches, currentSlots, categoryId, currentRound, ref matchNumber);
                    return;
                }

                int halfSlots = currentSlots / 2;
                for (int i = 0; i < halfSlots; i++)
                {
                    matches.Add(BuildMatch(
                        categoryId, null, null,
                        currentRound, matchNumber,
                        bracketPosition: i));
                    matchNumber++;
                }
                currentSlots = halfSlots;
                currentRound++;
            }

            EmitSemiFinalBlock(matches, null, categoryId, currentRound, ref matchNumber);
        }

        private static void EmitSemiFinalBlock(
            List<Match> matches,
            int?        byeTeamId,
            int         categoryId,
            int         semiFinalRound,
            ref int     matchNumber)
        {
            matches.Add(BuildMatch(
                categoryId, null, null,
                semiFinalRound, matchNumber,
                bracketPosition: 0));
            matchNumber++;

            int byeAndFinalRound = semiFinalRound + 1;

            matches.Add(BuildMatch(
                categoryId,
                sideA:           byeTeamId,
                sideB:           null,
                roundNumber:     byeAndFinalRound,
                matchNumber:     matchNumber,
                bracketPosition: 1));
            matchNumber++;

            matches.Add(BuildMatch(
                categoryId, null, null,
                byeAndFinalRound, matchNumber,
                bracketPosition: 0));
            matchNumber++;
        }

        private static Match BuildMatch(
            int  categoryId,
            int? sideA,
            int? sideB,
            int  roundNumber,
            int  matchNumber,
            int  bracketPosition) => new()
        {
            EventCategoryId  = categoryId,
            SideAId          = sideA,
            SideBId          = sideB,
            RoundNumber      = roundNumber,
            MatchNumber      = matchNumber,
            BracketPosition  = bracketPosition,
            Status           = MatchStatus.Upcoming,
            MatchVenue       = string.Empty,
            MatchDateTime    = default,
            CreatedAt        = DateTime.UtcNow
        };
    }
}