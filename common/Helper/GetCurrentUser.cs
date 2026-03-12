using System.Security.Claims;
using SportsManagementApp.Common.Exceptions;

namespace SportsManagementApp.Common.Helper;

public static class GetCurrentUser
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var id))
            throw new UnauthorizedAppException("Invalid user token.");

        return id;
    }
}