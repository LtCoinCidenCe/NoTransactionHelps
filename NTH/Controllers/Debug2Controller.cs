#if DEBUG
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;

namespace NTH.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Debug2Controller(SQLiteContext database) : ControllerBase
{
	private static HttpClient httpClient = new HttpClient();

	[HttpGet]
	[Route("httpAuth")]
	public async Task<IActionResult> HttpGo()
	{
		var asyncCall = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}/api/Login", new UserLoginDTO() { Username = "star", Password = "texas" });
		var jwt = await asyncCall.Content.ReadAsStringAsync();
		if (string.IsNullOrEmpty(jwt))
			throw new NTHException("httpAuth jwt is not received");
		return Ok("OK");
	}

	[HttpGet]
	[Route("GetUsers")]
	public List<UserID> GetUsers()
	{
		var users = database.Users
			// .AsSplitQuery()
			.AsSingleQuery()
			.AsNoTracking()
			.Include(x => x.DisplaynameHistory)
			.Include(x => x.UserRoleHistory)
			.Include(x => x.Contact)
			.ThenInclude(contact => contact.Author)
			.Include(x => x.Works)
			.ThenInclude(x => x.Video)
			.ToList();
		return users;
	}
}

#endif
