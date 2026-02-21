using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(AppConstants.ApiVersion, new()
                {
                    Title       = AppConstants.SwaggerTitle,
                    Version     = AppConstants.ApiVersion,
                    Description = AppConstants.SwaggerDescription
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}