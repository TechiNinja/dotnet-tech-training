using SportsManagementApp.Mappings;
using SportsManagementApp.Repositories.Implementations;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Services.Strategies;

namespace SportsManagementApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFixtureServices(this IServiceCollection services)
        {
            services.AddScoped<IFixtureStrategy, KnockoutFixtureStrategy>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFixtureService, FixtureService>();
            services.AddScoped<IMatchService, MatchService>();
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}