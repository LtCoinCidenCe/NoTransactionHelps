namespace NTH.Utilities.Middlewares;

public class RequestLimiter(RequestDelegate next)
{
    private static SemaphoreSlim semaphoreLimiter = new(20, 20);
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await semaphoreLimiter.WaitAsync();
            await next(context);
        }
        finally
        {
            semaphoreLimiter.Release();
        }
    }
}
