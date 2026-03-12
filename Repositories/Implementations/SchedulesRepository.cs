using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Participant;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class SchedulesRepository: ISchedulesRepository
    {
        private readonly AppDbContext _context;

        public SchedulesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MyScheduleDto>> GetUserScheduleAsync(int userId)
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
