using SportsManagementApp.Repositories.Implementations;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Extensions
{
    public static class RepositoryServiceExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRoleRepository, RolesRepository>();
            services.AddScoped<ISportRepository, SportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISchedulesRepository, SchedulesRepository>();
            services.AddScoped<ITeamsRepository, TeamsRepository>();
            services.AddScoped<IEventsRepository, EventsRepository>();
            services.AddScoped<IParticipantRegistrationRepository, ParticipantRegistrationRepository>();

            return services;
        }
    }
}
