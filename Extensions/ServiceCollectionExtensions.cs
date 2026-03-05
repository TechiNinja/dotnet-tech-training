using SportsManagementApp.Mappings;
using SportsManagementApp.Repositories;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Services.Strategies;

namespace SportsManagementApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository,         EventRepository>();
            services.AddScoped<IEventRequestRepository,  EventRequestRepository>();
            services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
            services.AddScoped<IMatchRepository,         MatchRepository>();
            services.AddScoped<IUserRepository,          UserRepository>();
            services.AddScoped<ITeamRepository,          TeamRepository>();

            services.AddScoped<IFixtureStrategy, KnockoutFixtureStrategy>();
            services.AddScoped<IFixtureStrategy, RoundRobinFixtureStrategy>();
            services.AddScoped<IFixtureStrategyResolver, FixtureStrategyResolver>();

            services.AddScoped<IEventService,    EventService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFixtureService,  FixtureService>();
            services.AddScoped<IMatchService,    MatchService>();

            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
