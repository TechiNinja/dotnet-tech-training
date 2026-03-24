using System.Net;
using System.Net.Cache;
using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Analytics;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public AnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminAnalyticsDto> GetAdminAnalyticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return new AdminAnalyticsDto
            {
                TotalEvents = await _context.Events.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(user => user.IsActive),
                Teams = await _context.Teams.CountAsync(),
                MatchesToday = await _context.Matches.CountAsync(match =>
                    match.MatchDateTime >= today && match.MatchDateTime < tomorrow)
            };
        }

        public async Task<OrganizerAnalyticsDto> GetOrganizerAnalyticsAsync(int organizerId)
        {
            return new OrganizerAnalyticsDto
            {
                MyEvents = await _context.Events.CountAsync(evt =>
                    evt.OrganizerId == organizerId),

                TotalRegistrations = await _context.ParticipantRegistrations.CountAsync(registration =>
                    registration.EventCategory!.Event!.OrganizerId == organizerId),

                TeamsRegistered = await _context.Teams.CountAsync(team =>
                    team.EventCategory!.Event!.OrganizerId == organizerId),

                LiveMatches = await _context.Matches.CountAsync(match =>
                    match.Status == MatchStatus.Live &&
                    match.EventCategory!.Event!.OrganizerId == organizerId)
            };
        }

        public async Task<OperationAnalyticsDto> GetOperationAnalyticsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return new OperationAnalyticsDto
            {
                TotalRequests = await _context.EventRequests.CountAsync(),
                TotalEvents = await _context.Events.CountAsync(),
                PendingRequests = await _context.EventRequests.CountAsync(request => request.Status == RequestStatus.Pending),
                MatchesToday = await _context.Matches.CountAsync(match =>
                    match.MatchDateTime == today )
            };
        }
    }
}



