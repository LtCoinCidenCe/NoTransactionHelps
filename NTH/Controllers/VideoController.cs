using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.Video;
using NTH.Utilities;
using SixLabors.ImageSharp;
using System.ComponentModel.DataAnnotations;

namespace NTH.Controllers;

[ApiController]
[Route("api/Video")]
#pragma warning disable CS9113 // 参数未读。
public class VideoController(ILogger<VideoController> logger, SQLiteContext database, [FromServices] RequestingUser requestingUser) : ControllerBase
#pragma warning restore CS9113 // 参数未读。
{
	[HttpGet, Authorize]
	[Route("AllAuthorizedVideo")]
	public IActionResult GetAllAuthorizedVideo()
	{
		var generousAuthors = database.Authors
			.Where(x => x.AllVideoAuthorized).Select(x => x.ID).ToList();
		var validVideos = database.Videos
			.Where(x => generousAuthors.Contains(x.AuthorID) || x.AuthorizedPerVideo)
			.Select(x => new
			{
				x.ID,
				x.Title,
				x.Introduction,
				x.YoutubePage,
				x.NiconicoID,
				x.BilibiliPage,
				x.AuthorID,
				x.AuthorizedPerVideo,
				x.UploadDate,
				x.StatusTranslation,
				x.StatusScripting,
				x.StatusHardSubbing,
				x.AdditionalRequirement,
				x.FinishedProductLink,
			});
		return Ok(validVideos);
	}

	[HttpPost, Authorize]
	public async Task<IActionResult> CreateNewVideo([FromBody] NewVideoDTO newVideoDTO)
	{
		var author = database.Authors.Where(x => x.ID == newVideoDTO.AuthorID).Select(x => new { x.ID, x.Name }).FirstOrDefault();
		if (author is null)
			return NotFound("Author not found");
		var newDate = DateTimeOffset.UtcNow;
		var newVideo = new VideoID()
		{
			ByUserAudit = requestingUser.UserID,
			AuthorID = author.ID,
			Title = newVideoDTO.Title,
			Introduction = newVideoDTO.Introduction,
			YoutubePage = newVideoDTO.YoutubePage,
			NiconicoID = newVideoDTO.NiconicoPage,
			BilibiliPage = newVideoDTO.BilibiliPage,
			UploadDate = newVideoDTO.UploadDate,
			AuthorizedPerVideo = newVideoDTO.AuthorizedPerVideo,
			AdditionalRequirement = newVideoDTO.AdditionalRequirement,
			FinishedProductLink = newVideoDTO.FinishedProductLink,
			CreationDate = newDate,
			UpdatedAt = newDate
		};
		// Thumbnail handling
		if (!string.IsNullOrWhiteSpace(newVideoDTO.ThumbnailType) && newVideoDTO.Thumbnail.Length >= 5)
		{
			Image image;
			try { image = Image.Load(newVideoDTO.Thumbnail); }
			catch (Exception) { return BadRequest("什么破图？"); }
			using (image)
			{
				image.Size.Deconstruct(out int x, out int y);
				Guid guid = Guid.CreateVersion7();
				var savedPath = Path.Join(VideoCookieAssetController.VideoIconPath, guid.ToString() + newVideoDTO.ThumbnailType);
				await System.IO.File.WriteAllBytesAsync(savedPath, newVideoDTO.Thumbnail);
				newVideo.ThumbnailGUID = guid;
				newVideo.ThumbnailChangeDate = DateTimeOffset.UtcNow;
			}
		}

		database.Videos.Add(newVideo);
		database.SaveChanges();

		return Ok(newVideoDTO);
	}

	[HttpPut, Authorize]
	[Route("{ID}/Thumbnail")]
	public async Task<IActionResult> SetVideoThumbnail(long ID, IFormFile file)
	{
		if (file.Length < 5 || file.Length > VideoID.MAX_THUMBNAIL_SIZE)
			return BadRequest();
		if (string.IsNullOrEmpty(Path.GetExtension(file.FileName)))
			return BadRequest("没后缀名啊");
		using Stream readStream = file.OpenReadStream();
		byte[] bytes1 = new byte[file.Length];
		await readStream.ReadExactlyAsync(bytes1);
		Image image;
		try { image = Image.Load(bytes1); }
		catch (Exception) { return BadRequest("什么破图？"); }
		using (image)
		{
			image.Size.Deconstruct(out int x, out int y);
			Guid guid = Guid.CreateVersion7();
			var savedPath = Path.Join(VideoCookieAssetController.VideoIconPath, guid.ToString() + Path.GetExtension(file.FileName).ToLower());
			await System.IO.File.WriteAllBytesAsync(savedPath, bytes1);

			DateTimeOffset newDate = DateTimeOffset.UtcNow;
			int updates = database.Videos.Where(z => z.ID == ID)
				.ExecuteUpdate(setter => setter
					.SetProperty(x => x.ThumbnailGUID, guid)
					.SetProperty(x => x.ThumbnailChangeDate, newDate));
			if (updates != 1)
				throw new NTHException("video executeupdate guard");
		}

		return Ok("OK");
	}

	[HttpGet, Authorize]
	[Route("AllVideo")]
	public IActionResult GetAllVideo()
	{
		return Ok(database.Videos.OrderByDescending(x => x.ID).Select(x => new
		{
			x.ID,
			x.Title,
			x.Introduction,
			x.YoutubePage,
			x.NiconicoID,
			x.BilibiliPage,
			x.AuthorID,
			x.AuthorizedPerVideo,
			x.UploadDate,
			x.StatusTranslation,
			x.StatusScripting,
			x.StatusHardSubbing,
			x.AdditionalRequirement,
			x.FinishedProductLink,
		}));
	}

	[HttpGet, Authorize]
	[Route("WorkStarted")]
	public IActionResult GetWorkStartedVideo()
	{
		return Ok(database.Videos.Where(x =>
			x.StatusTranslation > WorkStatus.NeverTouched || x.StatusTranslation < WorkStatus.Uploaded
		).Select(x => new
		{
			x.ID,
			x.Title,
			x.Introduction,
			x.YoutubePage,
			x.NiconicoID,
			x.BilibiliPage,
			x.AuthorID,
			x.AuthorizedPerVideo,
			x.UploadDate,
			x.StatusTranslation,
			x.StatusScripting,
			x.StatusHardSubbing,
			x.AdditionalRequirement,
			x.FinishedProductLink,
		}));
	}
}


/// <summary>
/// 希望所有的图片作为静态文件从GUID来获得
/// cookie验证
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("api/Video")]
public class VideoCookieAssetController : ControllerBase
{
	[HttpGet]
	[Route("Thumbnail/{IconID}")]
	[ResponseCache(Duration = 60 * 60 * 24 * 30)]
	public IActionResult GetIconByIconID([FromRoute] Guid IconID)
	{
		if (IconID == default)
			return NotFound();
		List<string> extensions = ["png", "jpg", "jpeg", "webp"];
		foreach (var ext in extensions)
		{
			var potentialFile = Path.Join(VideoIconPath, IconID.ToString() + '.' + ext);
			if (System.IO.File.Exists(potentialFile))
				return File(System.IO.File.Open(potentialFile, FileMode.Open), "image/" + ext);
		}
		return NotFound();
	}

	public static string VideoIconPath = null!;
}


public class NewVideoDTO
{
	public const int MAX_THUMBNAIL_SIZE = 3_000_000; // 3MB
	public const int MAX_URL = 200;

	#region Video itself
	[MaxLength(120)]
	public string Title { get; set; } = string.Empty;
	/// <summary>
	/// jpg png webp ...
	/// </summary>
	[MaxLength(6)]
	[AllowedValues("png", "jpg", "jpeg", "webp", "", null)]
	public string ThumbnailType { get; set; } = "";
	[MaxLength(MAX_THUMBNAIL_SIZE)]
	public byte[] Thumbnail { get; set; } = [];
	[MaxLength(3000)]
	public string Introduction { get; set; } = "";
	public long AuthorID { get; set; }
	[MaxLength(MAX_URL)]
	public string YoutubePage { get; set; } = string.Empty;
	[MaxLength(MAX_URL)]
	public string NiconicoPage { get; set; } = string.Empty;
	// If any author requests video to be translated for things here...
	[MaxLength(MAX_URL)]
	public string BilibiliPage { get; set; } = string.Empty;
	public DateTimeOffset UploadDate { get; set; } =
		new DateTimeOffset(1930, 1, 1, 0, 0, 0, TimeSpan.FromHours(0)); // that's before computer came into reality
	#endregion Video itself

	#region Authorization
	public bool AuthorizedPerVideo { get; set; } = false;
	#endregion Authorization

	#region Work details
	[MaxLength(800)]
	public string AdditionalRequirement { get; set; } = string.Empty;
	[MaxLength(MAX_URL)]
	public string FinishedProductLink { get; set; } = string.Empty;
	#endregion Work details
}
