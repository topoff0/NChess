using System.Security.Claims;

namespace Chess.API.Extensions;

public static class ClaimsPrincipalExtension
{
    private const string UserIdClaimType = "userId";

    public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
    {
        var claimValue = user.FindFirstValue(UserIdClaimType);

        return Guid.TryParse(claimValue, out userId);
    }
}
