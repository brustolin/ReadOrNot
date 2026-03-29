using System.Security.Claims;

namespace ReadOrNot.Api.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var rawValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new InvalidOperationException("The authenticated user identifier is missing.");

        return Guid.Parse(rawValue);
    }
}
