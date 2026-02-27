using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;
using SportsManagementApp.Data;
using SportsManagementApp.Extensions;
using SportsManagementApp.Middleware;
using SportsManagementApp.StringConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(3)
    ));

builder.Services.AddCors(options =>
    options.AddPolicy(AppConstants.ReactNativeCorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title       = AppConstants.SwaggerTitle;
    config.Description = AppConstants.SwaggerDescription;
    config.Version     = "v1";
});
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseOpenApi();
app.UseSwaggerUi(settings =>
{
    settings.Path = string.Empty;
    settings.DocumentPath = "/swagger/v1/swagger.json";
});

app.UseHttpsRedirection();
app.UseCors(AppConstants.ReactNativeCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();