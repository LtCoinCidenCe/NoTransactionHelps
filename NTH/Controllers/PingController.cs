using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;

namespace NTH.Controllers;

[ApiController]
[Route("api/Ping")]
public class PingController(SQLiteContext database) : ControllerBase
{
	[HttpGet]
	public IActionResult Ping()
	{
		return Ok("OK");
	}

	[HttpGet, Authorize]
	[Route("Authorized")]
	public IActionResult AuthorizedPing()
	{
		return Ok("OK");
	}

	[HttpGet]
	[Route("Started")]
	public IActionResult HealthCheck()
	{
		if (!InitialHealthCheckWindow.IsCompleted)
		{
			database.ProgramStarts.Add(new());
			database.SaveChanges();
		}

		return Ok("Healthy");
	}

	[HttpGet]
	[Route("Commit")]
	public string GetCommit()
	{
		return PipelineGitCommit.CurrentCommitHash;
	}

	private static readonly Task InitialHealthCheckWindow = Task.Delay(TimeSpan.FromSeconds(5));
}
