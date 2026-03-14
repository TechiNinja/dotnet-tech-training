using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using SportsManagementApp.Data;
using SportsManagementApp.Extensions;
using SportsManagementApp.Middleware;
using SportsManagementApp.Middlewares;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)
    ));

builder.Services.AddCors(options =>
    options.AddPolicy(AppConstants.ReactNativeCorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.Title       = AppConstants.SwaggerTitle;
    config.Description = AppConstants.SwaggerDescription;
    config.Version     = "v1";

    config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
    {
        Type        = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name        = "Authorization",
        In          = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    config.OperationProcessors.Add(
        new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT")
    );
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApplicationServices();
builder.Services.AddFixtureServices();
builder.Services.AddRepositories();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseOpenApi();
app.UseSwaggerUi(settings =>
{
    settings.Path         = string.Empty;
    settings.DocumentPath = "/swagger/v1/swagger.json";
});

// app.UseHttpsRedirection();
app.UseCors(AppConstants.ReactNativeCorsPolicy);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();