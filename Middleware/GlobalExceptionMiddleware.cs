using SportsManagementApp.StringConstants;
using System.Net;
using System.Text.Json;

namespace SportsManagementApp.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppConstants.UnhandledExceptionLog,
                    context.Request.Method, context.Request.Path);
                await WriteErrorAsync(context, ex);
            }
        }

        private static Task WriteErrorAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                ArgumentNullException       => (HttpStatusCode.BadRequest,          AppConstants.RequiredValueMissing),
                ArgumentException           => (HttpStatusCode.BadRequest,          ex.Message),
                InvalidOperationException   => (HttpStatusCode.UnprocessableEntity, ex.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized,        AppConstants.UnauthorizedAccess),
                _                           => (HttpStatusCode.InternalServerError, AppConstants.UnexpectedError)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            var body = JsonSerializer.Serialize(new
            {
                statusCode = (int)statusCode,
                message    = message,
                traceId    = context.TraceIdentifier
            });

            return context.Response.WriteAsync(body);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}