using Microsoft.AspNetCore.Mvc.Filters;

namespace NTH.Controllers.Filters;

/// <summary>
/// To apply LiaoTian message lock, keep the ID and datetime sequential
/// </summary>
public class NewMessageFilterAttribute : ActionFilterAttribute
{
	public static readonly SemaphoreSlim theLock = new(1, 1);
	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		await theLock.WaitAsync();
		try
		{
			await next();
		}
		finally
		{
			theLock.Release();
		}
	}
}
