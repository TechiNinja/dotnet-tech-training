using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RolesService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISchedulesService, SchedulesService>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<IEventsService, EventsService>();
            services.AddScoped<IParticipantRegistrationService, ParticipantRegistrationService>();
            services.AddScoped<IEventRequestService ,EventRequestService>();
            services.AddScoped<INotificationService ,NotificationService>();
            services.AddScoped<IOperationsService ,OperationsService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();

            return services;
        }
    }
}
