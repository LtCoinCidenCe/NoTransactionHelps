namespace NTH.Utilities.Middlewares;

public class RequestLimiter(RequestDelegate next)
{
    private static SemaphoreSlim semaphoreLimiter = new(20, 20);
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            semaphoreLimiter.Wait();
            await next(context);
        }
        finally
        {
            semaphoreLimiter.Release();
        }
    }
}
