using Microsoft.EntityFrameworkCore;
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
builder.Services.AddSwaggerDocs();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(AppConstants.SwaggerEndpoint, AppConstants.SwaggerDisplayName);
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors(AppConstants.ReactNativeCorsPolicy);
app.MapControllers();
app.Run();