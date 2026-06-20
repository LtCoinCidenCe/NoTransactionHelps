namespace NTH.Utilities.Middlewares;

public class HomepageGuide(RequestDelegate next)
{
	private static readonly string indexHTML;

	static HomepageGuide()
	{
		indexHTML = File.ReadAllText("wwwroot/index.html");
		if (indexHTML.Length < 10)
		{
			throw new NTHException("indexHTML not loaded correctly");
		}
	}

	public async Task InvokeAsync(HttpContext context)
	{
		await next(context);
		if (context.Response.HasStarted)
		{
			return;
		}
		await ReplyIndexHTML(context);
	}

	private async Task ReplyIndexHTML(HttpContext context)
	{
		context.Response.StatusCode = StatusCodes.Status200OK;
		context.Response.ContentType = "text/html";
		await context.Response.WriteAsync(indexHTML);
	}
}
