using System.Text.Json;
using SportsManagementApp.Common.Exceptions;

namespace SportsManagementApp.Common.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            await HandleAppException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownException(context, ex);
        }
    }

    private async Task HandleAppException(HttpContext context, AppException ex)
    {
        _logger.LogWarning(ex,
            "Handled application exception {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            title = ex.Title,
            detail = ex.Detail,
            status = ex.StatusCode,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleUnknownException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex,
            "Unhandled server exception {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            title = "Server Error",
            detail = _environment.IsDevelopment()
                ? ex.Message
                : "Something went wrong.",
            status = StatusCodes.Status500InternalServerError,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}