using System.Security.Claims;

namespace NTH.Utilities;

public static class ControllerHelper
{
    public static bool CheckUserClaimsID(ClaimsPrincipal user, long ID)
    {
        string? identity = user.FindFirstValue("aud");
        if (identity is null)
            return false;
        if (identity.Length < 3)
            return false;
        long.TryParse(identity.Substring(2), out var provedID);
        if (provedID != ID)
            return false;
        return true;
    }
}
