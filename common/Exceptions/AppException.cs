using Microsoft.AspNetCore.Http;

namespace SportsManagementApp.Common.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(int statusCode, string title, string? detail = null)
        : base(detail ?? title)
    {
        StatusCode = statusCode;
        Title = title;
        Detail = detail;
    }

    public int StatusCode { get; }
    public string Title { get; }
    public string? Detail { get; }
}

    public sealed class ValidationAppException(string detail)
        : AppException(StatusCodes.Status400BadRequest, "Validation Error", detail);

public sealed class UnauthorizedAppException(string detail)
    : AppException(StatusCodes.Status401Unauthorized, "Unauthorized", detail);

public sealed class ForbiddenAppException(string detail)
    : AppException(StatusCodes.Status403Forbidden, "Forbidden", detail);

public sealed class NotFoundAppException(string detail)
    : AppException(StatusCodes.Status404NotFound, "Not Found", detail);

public sealed class ConflictAppException(string detail)
    : AppException(StatusCodes.Status409Conflict, "Conflict", detail);