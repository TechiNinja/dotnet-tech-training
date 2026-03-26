using SportsManagementApp.Repositories.Implementations;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Extensions
{
    public static class RepositoryServiceExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRoleRepository, RolesRepository>();
            services.AddScoped<ISportRepository, SportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISchedulesRepository, SchedulesRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<IParticipantRegistrationRepository, ParticipantRegistrationRepository>();
            services.AddScoped<IEventRequestRepository, EventRequestRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();


            return services;
        }
    }
}
