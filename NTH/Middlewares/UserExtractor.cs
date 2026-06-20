using System.Security.Claims;
using NTH.Models.User;

namespace NTH.Middlewares;

public class UserExtractor(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context, RequestingUser data)
	{
		string? identity = context.User.FindFirstValue("aud");
		if (!string.IsNullOrEmpty(identity) && identity.Length > 2)
		{
			if (identity.StartsWith("sa"))
				data.UserRole = UserRoleDTO.SystemAdministrator;
			long.TryParse(identity.Substring(2), out data.UserID);
		}
		await next(context);
	}
}

public class RequestingUser
{
	public long UserID;
	public UserRoleDTO UserRole;
}
