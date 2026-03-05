using SportsManagementApp.Exceptions;
using SportsManagementApp.StringConstants;
using System.Net;
using System.Text.Json;

namespace SportsManagementApp.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate                    _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppConstants.UnhandledExceptionLog,
                    context.Request.Method, context.Request.Path);
                await WriteErrorResponseAsync(context, ex);
            }
        }

        private static Task WriteErrorResponseAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ExceptionStatusMapper.Map(ex);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                statusCode = (int)statusCode,
                message,
                traceId = context.TraceIdentifier
            }));
        }
    }
}
