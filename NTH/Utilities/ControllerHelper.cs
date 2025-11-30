using System.Security.Claims;

namespace NTH.Utilities;

public static class ControllerHelper
{
    /// <summary>
    /// Check if the jwt is issued for the user with ID.
    /// <code>ControllerHelper.CheckUserClaimsID(User, ID)</code>
    /// </summary>
    /// <param name="user">Controller Property User</param>
    /// <param name="ID">the user ID</param>
    /// <returns>true if the jwt aud data contains the ID</returns>
    public static bool CheckUserClaimsID(ClaimsPrincipal user, long ID)
    {
        string? identity = user.FindFirstValue("aud");
        if (identity is null)
            return false;
        if (identity.Length < 3)
            return false;
        if (identity.StartsWith("sa"))
            return true;
        long.TryParse(identity.Substring(2), out var provedID);
        if (provedID != ID)
            return false;
        return true;
    }
}
