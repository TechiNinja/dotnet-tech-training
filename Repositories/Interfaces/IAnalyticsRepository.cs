using SportsManagementApp.Data.DTOs.Analytics;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IAnalyticsRepository
    {
        Task<AdminAnalyticsDto> GetAdminAnalyticsAsync();
        Task<OrganizerAnalyticsDto> GetOrganizerAnalyticsAsync(int organizerId);
        Task<OperationAnalyticsDto> GetOperationAnalyticsAsync();
    }
}