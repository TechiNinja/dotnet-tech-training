using SportsManagementApp.Constants;
using SportsManagementApp.Exceptions;
using System.Net;
using System.Text.Json;

namespace SportsManagementApp.Middlewares
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    StringConstant.UnhandledException + " {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await WriteErrorAsync(context, exception);
            }
        }

        private static Task WriteErrorAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, exception.Message),
                UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
                _ => (HttpStatusCode.InternalServerError, StringConstant.InternalServerError),
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                statusCode,
                message,
                traceId = context.TraceIdentifier
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
