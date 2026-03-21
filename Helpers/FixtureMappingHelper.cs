using AutoMapper;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services
{
    public static class FixtureMappingHelper
    {
        public static IEnumerable<FixtureResponseDto> MapFixtures(
            IEnumerable<Match> matches,
            EventCategory category,
            IMapper mapper)
        {
            var teamNames = category.Teams
                .ToDictionary(t => t.Id, t => t.Name);

            var userNames = category.EventRegistrations
                .Where(r => r.User != null)
                .ToDictionary(r => r.UserId, r => r.User!.FullName);

            return matches.Select(m =>
            {
                var dto = mapper.Map<FixtureResponseDto>(m);
                dto.SideAName = ResolveSideName(m.SideAId, category.Format, teamNames, userNames);
                dto.SideBName = ResolveSideName(m.SideBId, category.Format, teamNames, userNames);
                return dto;
            });
        }

        private static string ResolveSideName(
            int? sideId,
            MatchFormat format,
            Dictionary<int, string> teamNames,
            Dictionary<int, string> userNames)
        {
            if (!sideId.HasValue) return "BYE";

            if (format == MatchFormat.Doubles && teamNames.TryGetValue(sideId.Value, out var teamName))
                return teamName;

            return userNames.TryGetValue(sideId.Value, out var userName) ? userName : $"#{sideId}";
        }
    }
}