using Microsoft.AspNetCore.Mvc.Filters;

namespace NTH.Filters;

public class ResourceFilterAttribute<T> : ActionFilterAttribute
{
	private static readonly SemaphoreSlim theLock = new(1, 1);
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
