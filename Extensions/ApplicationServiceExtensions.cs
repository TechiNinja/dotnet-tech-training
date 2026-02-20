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
            services.AddScoped<IParticipantService, ParticipantService>();

            return services;
        }
    }
}
