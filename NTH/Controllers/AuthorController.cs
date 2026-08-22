using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.Author;
using NTH.Models.Work;
using NTH.Services;
using SixLabors.ImageSharp;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace NTH.Controllers;

[Authorize, ApiController, Route("api/Author")]
#pragma warning disable CS9113 // 参数未读。
public class AuthorController(ILogger<AuthorController> logger, SQLiteContext database, AuthorService authorService, [FromServices] RequestingUser requestingUser)
#pragma warning restore CS9113 // 参数未读。
: ControllerBase
{
	[HttpGet]
	public IActionResult GetAllAuthors()
	{
		//var data = database.Authors.Include(x => x.Contact)
		//	.Select(author => new
		//	{
		//		// This is to exclude Icon bytes, if the Icon bytes is in the table
		//		author.ID,
		//		author.Name,
		//		author.YoutubeHomePage,
		//		author.NiconicoHomePage,
		//		author.BilibiliHomePage,
		//		author.TwitterHomePage,
		//		author.AuthorizedPerVideo,
		//		author.AllVideoAuthorized,
		//		author.AuthorizationChangeDate,
		//		author.AdditionalRequirements,
		//		author.AdditionalRequirementsChangeDate,
		//		author.CreationDate,
		//		author.Contact
		//	}).ToList();

		//return Ok(data.Select(x => new {
		//	x.ID,
		//	x.Name,
		//	x.YoutubeHomePage,
		//	x.NiconicoHomePage,
		//	x.BilibiliHomePage,
		//	x.TwitterHomePage,
		//	x.AuthorizedPerVideo,
		//	x.AllVideoAuthorized,
		//	x.AuthorizationChangeDate,
		//	x.AdditionalRequirements,
		//	x.AdditionalRequirementsChangeDate,
		//	x.CreationDate,
		//	ContactUserID = x.ContactUserIDraw.FirstOrDefault()
		//}));

		// This is somehow... huge if someone has changes contact multiple times
		// But how many times you really need to change people contacting??
		return Ok(database.Authors.Include(x => x.Contact).Select(x => new
		{
			x.ID,
			x.Name,
			x.AuthorIconID,
			x.IconChangeDate,
			x.YoutubeHomePage,
			x.NiconicoHomePage,
			x.BilibiliHomePage,
			x.TwitterHomePage,
			x.AuthorizedPerVideo,
			x.AllVideoAuthorized,
			x.AuthorizationChangeDate,
			x.AdditionalRequirements,
			x.AdditionalRequirementsChangeDate,
			x.CreationDate,
			x.Contact
		}));
	}

	/// <summary>
	/// Register a new author. Name should be unique but too lazy to check with mutex.
	/// </summary>
	/// <returns></returns>
	[HttpPost]
	public ActionResult<AuthorID> CreateNewAuthor(NewAuthorDTO newAuthorDTO)
	{
		bool existing = database.Authors.AsNoTracking().Any(x => x.Name == newAuthorDTO.Name);
		if (existing)
			return BadRequest();
		AuthorID author = newAuthorDTO.ToDBModel();
		author.ByUserAudit = requestingUser.UserID;
		database.Authors.Add(author);
		database.SaveChanges();
		return CreatedAtAction(nameof(CreateNewAuthor), author);
	}

	[HttpPut]
	[Route("{authorID}/Requirements")]
	public ActionResult<AdditionalRequirementsHistory> SetAuthorRequirements(long authorID, [FromBody, MaxLength(800)] string requirements)
	{
		// avoid fetching the author row with its big Icon
		bool existing = database.Authors.Any(x => x.ID == authorID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;
		var newHistory = new AdditionalRequirementsHistory()
		{
			AuthorID = authorID,
			ByUserAudit = requestingUser.UserID,
			TensaiRequirements = requirements,
			CreationDate = timeNow
		};
		database.AdditionalRequirementsHistories.Add(newHistory);
		database.SaveChanges();
		database.Authors
			.Where(x => x.ID == authorID)
			.ExecuteUpdate(setter => setter
				.SetProperty(t => t.AdditionalRequirements, requirements)
				.SetProperty(t => t.AdditionalRequirementsChangeDate, timeNow));
		return Ok(newHistory);
	}

	[HttpPut]
	[Route("{authorID}/Authorization")]
	public ActionResult<AuthorizationChangeHistory> SetAuthorization(long authorID, [FromBody] AuthorizationChangeDTO newAuth)
	{
		// avoid fetching the author row with its big Icon
		bool existing = database.Authors.Any(x => x.ID == authorID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;
		var newHistory = new AuthorizationChangeHistory()
		{
			ByUserAudit = requestingUser.UserID,
			AuthorizedPerVideo = newAuth.AuthorizedPerVideo,
			AllVideoAuthorized = newAuth.AllVideoAuthorized,
			AuthorID = authorID,
			CreationDate = timeNow
		};
		database.AuthorizationChangeHistories.Add(newHistory);
		database.SaveChanges();
		database.Authors
			.Where(x => x.ID == authorID)
			.ExecuteUpdate(setter => setter
				.SetProperty(t => t.AllVideoAuthorized, newAuth.AllVideoAuthorized)
				.SetProperty(t => t.AuthorizedPerVideo, newAuth.AuthorizedPerVideo)
				.SetProperty(t => t.AuthorizationChangeDate, timeNow));
		return Ok(newHistory);
	}

	[HttpPut]
	[Route("{authorID}/Contact")]
	public ActionResult<WorkContact> SetContact(long authorID, [FromBody] long userID)
	{
		var atr = database.Authors
			.AsNoTracking()
			.Include(x => x.Contact)
			.Where(x => x.ID == authorID)
			.Select(x => new { x.ID, x.Name, x.Contact, x.CreationDate })
			.FirstOrDefault();
		if (atr is null)
			return NotFound();
		bool existing = database.Users.Any(x => x.ID == userID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;

		var newContact = new WorkContact()
		{
			ByUserAudit = requestingUser.UserID,
			ChangeDate = timeNow,
			UserID = userID,
			AuthorID = authorID
		};
		database.WorkContacts.Add(newContact);
		database.SaveChanges();
		return Ok(newContact);
	}

	[HttpPut]
	[Route("{ID}/Icon")]
	public IActionResult SetAuthorIcon([FromRoute] long ID, IFormFile icon)
	{
		if (icon.Length < 5 || icon.Length > AuthorIconHistory.MAX_ICON_SIZE)
			return BadRequest("你想害我的库？");
		if (!database.Authors.Any(x => x.ID == ID))
			return BadRequest("查无此作者");
		Stream readStream = icon.OpenReadStream();

		Image image;
		try { image = Image.Load(readStream); }
		catch (Exception) { return BadRequest("什么破图？"); }
		using (image)
		{
			image.Size.Deconstruct(out int x, out int y);
			if (x != y)
				return BadRequest("不是正方形图片");
			if (x < 25)
				return BadRequest("太小");
			if (x > 800)
				return BadRequest("太大");
			using MemoryStream pngStream = new();
			image.SaveAsPng(pngStream);
			byte[] bytes = pngStream.ToArray();
			if (bytes.Length > AuthorIconHistory.MAX_ICON_SIZE)
				return BadRequest();
			DateTimeOffset newDate = DateTimeOffset.UtcNow;
			var historyItem = new AuthorIconHistory
			{
				ByUserAudit = requestingUser.UserID,
				AuthorID = ID,
				Icon = bytes,
				CreationDate = newDate,
			};
			database.AuthorIconHistories.Add(historyItem);
			database.SaveChanges();
			database.Authors.Where(x => x.ID == ID)
				.ExecuteUpdate(setter => setter
					.SetProperty(a => a.AuthorIconID, historyItem.GUID)
					.SetProperty(u => u.IconChangeDate, newDate));
			return Ok(historyItem.GUID);
		}
	}

	[HttpPost]
	[Route("dlp")]
	public IActionResult YTDLPOnAuthor([FromBody] int authorNicoID)
	{
		var worker = new Process();
		worker.StartInfo.FileName = "yt-dlp";
		worker.StartInfo.Arguments = "--write-thumbnail --write-description --write-info-json --no-download --no-cache-dir --force-overwrites https://www.nicovideo.jp/user/118691209";
		worker.StartInfo.WorkingDirectory = Program.dlpPath;
		worker.StartInfo.RedirectStandardOutput = true;
		worker.Start();
		worker.WaitForExit(TimeSpan.FromMinutes(1.5));
		var sr = worker.StandardOutput.ReadToEnd();
		return Ok(sr);
	}
}

public class AuthorizationChangeDTO
{
	public required bool AuthorizedPerVideo { get; set; }
	public required bool AllVideoAuthorized { get; set; }
}

/// <summary>
/// For minimal requirement, write only Name
/// </summary>
public class NewAuthorDTO
{
	[MaxLength(30)]
	public required string Name { get; set; }
	[MaxLength(200)]
	public string YoutubeHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string NiconicoHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string BilibiliHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string TwitterHomePage { get; set; } = string.Empty;
	public bool AuthorizedPerVideo { get; set; } = false;
	public bool AllVideoAuthorized { get; set; } = false;
	[MaxLength(800)]
	public string TensaiRequirement { get; set; } = string.Empty;
}

public static class NewAuthorDTOExtension
{
	/// <summary>
	/// be aware this returned object contains AdditionalRequirementsHistory and AuthorizationChangeHistory
	/// </summary>
	/// <returns></returns>
	public static AuthorID ToDBModel(this NewAuthorDTO newAuthorDTO)
	{
		var datetime = DateTimeOffset.UtcNow;
		var newAuthor = new AuthorID()
		{
			Name = newAuthorDTO.Name,
			YoutubeHomePage = newAuthorDTO.YoutubeHomePage,
			NiconicoHomePage = newAuthorDTO.NiconicoHomePage,
			BilibiliHomePage = newAuthorDTO.BilibiliHomePage,
			TwitterHomePage = newAuthorDTO.TwitterHomePage,
			AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
			AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
			AuthorizationChangeDate = datetime,
			AdditionalRequirements = newAuthorDTO.TensaiRequirement,
			AdditionalRequirementsChangeDate = datetime,
			CreationDate = datetime
		};
		newAuthor.AdditionalRequirementsHistory.Add(new()
		{
			CreationDate = datetime,
			TensaiRequirements = newAuthorDTO.TensaiRequirement
		});
		newAuthor.AuthorizationChangeHistory.Add(new()
		{
			AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
			AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
			CreationDate = datetime
		});
		return newAuthor;
	}
}
