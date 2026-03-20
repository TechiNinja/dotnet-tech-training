using AutoMapper;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public sealed class KnockoutFixtureStrategy : IFixtureStrategy
    {
        private readonly IMapper _mapper;

        public KnockoutFixtureStrategy(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TournamentType TournamentType => TournamentType.Knockout;

        public List<Match> Generate(List<int?> sideIds, int categoryId)
        {
            var matches = new List<Match>();
            int matchNumber = 1;

            var teams = sideIds
                .Where(id => id.HasValue)
                .OrderBy(_ => Random.Shared.Next())
                .Select(id => id!.Value)
                .ToList();

            if (teams.Count < 2) return matches;

            if (teams.Count % 2 == 0)
                BuildEvenRound(matches, teams, categoryId, roundNumber: 1, ref matchNumber);
            else
                BuildOddRound(matches, teams, categoryId, roundNumber: 1, ref matchNumber);

            return matches;
        }

        private void BuildEvenRound(
            List<Match> matches,
            List<int> teams,
            int categoryId,
            int roundNumber,
            ref int matchNumber)
        {
            int pairs = teams.Count / 2;

            for (int i = 0; i < pairs; i++)
            {
                matches.Add(NewMatch(categoryId, teams[i * 2], teams[i * 2 + 1], roundNumber, matchNumber, i));
                matchNumber++;
            }

            BuildPlaceholderRounds(matches, pairs, categoryId, roundNumber + 1, ref matchNumber);
        }

        private void BuildOddRound(
            List<Match> matches,
            List<int> teams,
            int categoryId,
            int roundNumber,
            ref int matchNumber)
        {
            int byeTeamId = teams[0];
            var remaining = teams.Skip(1).ToList();
            int pairs = remaining.Count / 2;

            for (int i = 0; i < pairs; i++)
            {
                matches.Add(NewMatch(categoryId, remaining[i * 2], remaining[i * 2 + 1], roundNumber, matchNumber, i));
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
                    matches.Add(NewMatch(categoryId, null, null, currentRound, matchNumber, i));
                    matchNumber++;
                }
                currentSlots = halfSlots;
                currentRound++;
            }

            EmitSemiFinalBlock(matches, byeTeamId, categoryId, currentRound, ref matchNumber);
        }

        private void BuildPlaceholderRounds(
            List<Match> matches,
            int slotCount,
            int categoryId,
            int roundNumber,
            ref int matchNumber)
        {
            if (slotCount <= 1) return;

            if (slotCount % 2 != 0)
            {
                BuildPlaceholderOddRounds(matches, slotCount, categoryId, roundNumber, ref matchNumber);
                return;
            }

            int pairs = slotCount / 2;
            for (int i = 0; i < pairs; i++)
            {
                matches.Add(NewMatch(categoryId, null, null, roundNumber, matchNumber, i));
                matchNumber++;
            }

            BuildPlaceholderRounds(matches, pairs, categoryId, roundNumber + 1, ref matchNumber);
        }

        private void BuildPlaceholderOddRounds(
            List<Match> matches,
            int slotCount,
            int categoryId,
            int roundNumber,
            ref int matchNumber)
        {
            int pairs = (slotCount - 1) / 2;

            for (int i = 0; i < pairs; i++)
            {
                matches.Add(NewMatch(categoryId, null, null, roundNumber, matchNumber, i));
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
                    matches.Add(NewMatch(categoryId, null, null, currentRound, matchNumber, i));
                    matchNumber++;
                }
                currentSlots = halfSlots;
                currentRound++;
            }

            EmitSemiFinalBlock(matches, null, categoryId, currentRound, ref matchNumber);
        }

        private void EmitSemiFinalBlock(
            List<Match> matches,
            int? byeTeamId,
            int categoryId,
            int semiFinalRound,
            ref int matchNumber)
        {
            matches.Add(NewMatch(categoryId, null, null, semiFinalRound, matchNumber, 0));
            matchNumber++;

            int finalRound = semiFinalRound + 1;
            matches.Add(NewMatch(categoryId, byeTeamId, null, finalRound, matchNumber, 1));
            matchNumber++;
            matches.Add(NewMatch(categoryId, null, null, finalRound, matchNumber, 0));
            matchNumber++;
        }

        private Match NewMatch(
            int categoryId,
            int? sideA,
            int? sideB,
            int roundNumber,
            int matchNumber,
            int bracketPosition)
        {
            var dto = new MatchCreateDto
            {
                EventCategoryId = categoryId,
                SideAId = sideA,
                SideBId = sideB,
                RoundNumber = roundNumber,
                MatchNumber = matchNumber,
                BracketPosition = bracketPosition
            };
            return _mapper.Map<Match>(dto);
        }
    }
}