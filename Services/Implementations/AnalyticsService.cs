using SportsManagementApp.Constants;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public async Task<object> GetAnalyticsAsync(int userId, string role)
        {
            return role switch
            {
                RoleConstants.Admin => await _analyticsRepository.GetAdminAnalyticsAsync(),
                RoleConstants.Operation => await _analyticsRepository.GetOperationAnalyticsAsync(),
                RoleConstants.Organizer => await _analyticsRepository.GetOrganizerAnalyticsAsync(userId),
                _ => throw new UnauthorizedException("Analytics not available for this role")
            };
        }
    }
}
