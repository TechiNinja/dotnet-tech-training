using System.Security.Claims;
using SportsManagementApp.Exceptions;

namespace SportsManagementApp.Helper;

public static class UserHelper
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var id))
            throw new UnauthorizedException("Invalid user token.");

        return id;
    }
}