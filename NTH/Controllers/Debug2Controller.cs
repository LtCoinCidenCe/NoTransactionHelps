#if DEBUG
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.dlpJSONs;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;
using SixLabors.ImageSharp;
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
		var dlpFiles = Directory.EnumerateFiles(YtdlpInstanceService.dlpPath)
			.Select(x => new VideoNicoProcessingInfo { File = Path.GetFileName(x), FullPath = x }).ToList();
		foreach (var file in dlpFiles)
		{
			try
			{
				if (file.FullPath.EndsWith(".info.json", StringComparison.OrdinalIgnoreCase))
				{
					if (file.File.StartsWith("NA ["))
						continue;
					using var jsonStream = System.IO.File.OpenRead(file.FullPath);
					var theObject = await JsonSerializer.DeserializeAsync<VideoNicoInfo>(jsonStream);
					file.Info = theObject;
				}
				else if (file.FullPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
				{
					using var imageStream = System.IO.File.OpenRead(file.FullPath);
					Image.Load(imageStream);
					file.ImageBytes = await System.IO.File.ReadAllBytesAsync(file.FullPath);
				}
			}
			catch (Exception)
			{
			}
		}

		var groupedFiles = dlpFiles.GroupBy(x => x.File.Substring(0, 10));
		var cleanFiles = groupedFiles.Where(x => !x.Key.StartsWith("NA ["));

		Func<IGrouping<string, VideoNicoProcessingInfo>, VideoNicoProcessingInfo> selector = x =>
		{
			var bone = new VideoNicoProcessingInfo() { File = "", FullPath = "" };
			foreach (VideoNicoProcessingInfo file in x)
			{
				bone.File = file.File;
				bone.FullPath = file.FullPath;
				if (file.Info is not null)
					bone.Info = file.Info;
				if (file.ImageBytes.Length > 4)
					bone.ImageBytes = file.ImageBytes;
			}
			return bone;
		};
		var videos = cleanFiles.Select(selector).ToList();
		return Ok("OK");
	}

	public class VideoNicoProcessingInfo
	{
		public required string File;
		public required string FullPath;
		public VideoNicoInfo? Info;
		public byte[] ImageBytes = [];
	}
}

#endif
