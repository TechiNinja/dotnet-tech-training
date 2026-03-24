namespace SportsManagementApp.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<object> GetAnalyticsAsync(int userId, string role);
    }
}


