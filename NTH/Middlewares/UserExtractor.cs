using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using NTH.Models.User;

namespace NTH.Middlewares;

public class UserExtractor(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context, RequestingUser data, IMemoryCache cache)
	{
		string? identity = context.User.FindFirstValue("aud");
		long.TryParse(identity, out data.UserID);
		if (cache.TryGetValue<UserRoleDTO>($"UserRole{data.UserID}", out var role))
		{
			data.UserRole = role;
		}

		var roleStr = context.User.FindFirstValue(ClaimTypes.Role);
		if (int.TryParse(roleStr, out var roleInt))
		{
			if (roleInt == 0x7f_ff_ff_ff)
				data.UserRole = UserRoleDTO.God;
			else if (roleInt < 2 * (int)UserRoleDTO.SystemAdministrator && roleInt >= 0)
				data.UserRole = (UserRoleDTO)roleInt;
			// anything else would be illegal
		}

		await next(context);
	}
}

public class RequestingUser
{
	public long UserID;
	public UserRoleDTO UserRole;
}
