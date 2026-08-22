#if DEBUG
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.dlpJSONs;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;
using System.Text.Json;

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

	[HttpGet]
	[Route("TryParseJson")]
	public async Task<IActionResult> TryParse()
	{
		var filename = @"D:\VideoProjects\轴\information爬虫\dlpdata\【voiceroid劇場】この中で一番付き合いたいと思ってる女の子は誰でしょうか！！！ [sm44910136].info.json";
		var extended = filename.Split(' ').LastOrDefault();
		if (string.IsNullOrEmpty(extended))
			throw new NTHException("not valid truth json file");
		var jsonStream = System.IO.File.OpenRead(filename);
		var theObject = await JsonSerializer.DeserializeAsync<VideoNicoTruth>(jsonStream);
		return Ok("OK");
	}
}

#endif
