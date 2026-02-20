using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class ParticipantRepository: IParticipantRepository
    {
        private readonly AppDbContext _context;

        public ParticipantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MyEventsDto>> GetMyEventsAsync(int userId)
        {
            return await _context.ParticipantRegistrations
                .Where(registration => registration.UserId == userId && registration.EventCategory != null && registration.EventCategory.Event != null)
                .Select(registration => new MyEventsDto
                {
                    EventId = registration.EventCategory!.EventId,
                    EventName = registration.EventCategory!.Event!.Name,
                    StartDate = registration.EventCategory.Event.StartDate,
                    EndDate = registration.EventCategory.Event.EndDate
                })
                .ToListAsync();
        }

        public async Task<List<MyTeamDto>> GetMyTeamsAsync(int userId)
        {
            return await _context.TeamMembers
                .Where(member => member.UserId == userId)
                .Select(member => new MyTeamDto
                {
                    TeamId = member.TeamId,
                    TeamName = member.Team != null ? member.Team.Name : "N/A",
                    Category = member.Team != null && member.Team.EventCategory != null
                        ? $"{member.Team.EventCategory.Gender} {member.Team.EventCategory.Format}"
                        : "N/A",
                    EventName = member.Team != null
                        && member.Team.EventCategory != null
                        && member.Team.EventCategory.Event != null
                            ? member.Team.EventCategory.Event.Name
                            : "N/A",
                })
                .ToListAsync();
        }

        public async Task<List<MyScheduleDto>> GetMyScheduleAsync(int userId)
        {
            var teamIds = await _context.TeamMembers
                .Where(member => member.UserId == userId)
                .Select(member => member.TeamId)
                .ToListAsync();

            return await _context.Matches
                .Where(match =>
                    (match.SideAId != null && teamIds.Contains(match.SideAId.Value)) ||
                    (match.SideBId != null && teamIds.Contains(match.SideBId.Value)))
                .Select(match => new MyScheduleDto
                {
                    MatchId = match.Id,
                    MatchDateTime = match.MatchDateTime,
                    Venue = match.MatchVenue,
                    SideA = match.SideAId != null ? match.SideAId.Value.ToString() : "N/A",
                    SideB = match.SideBId != null ? match.SideBId.Value.ToString() : "N/A",
                    ScoreA = match.MatchSets.Sum(set => set.ScoreA),
                    ScoreB = match.MatchSets.Sum(set => set.ScoreB),
                    EventName = match.EventCategory != null && match.EventCategory.Event != null
                        ? match.EventCategory.Event.Name
                        : "N/A"
                })
                .ToListAsync();
        }
    }
}
