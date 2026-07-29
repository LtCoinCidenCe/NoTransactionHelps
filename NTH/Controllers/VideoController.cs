using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.Video;
using NTH.Utilities;

namespace NTH.Controllers;

[ApiController]
[Route("api/Video")]
public class VideoController(ILogger<VideoController> logger, SQLiteContext database, [FromServices] RequestingUser requestingUser) : ControllerBase
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
				x.NiconicoPage,
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
	public IActionResult CreateNewVideo([FromBody] NewVideoDTO newVideoDTO)
	{
		var author = database.Authors.Where(x => x.ID == newVideoDTO.AuthorID).Select(x => new { x.ID, x.Name }).FirstOrDefault();
		if (author is null)
			return NotFound("Author not found");
		var newVideo = new VideoID()
		{
			ByUserAudit = requestingUser.UserID,
			AuthorID = author.ID,
			Title = newVideoDTO.Title,
			ThumbnailType = newVideoDTO.ThumbnailType,
			Thumbnail = newVideoDTO.Thumbnail,
			Introduction = newVideoDTO.Introduction,
			YoutubePage = newVideoDTO.YoutubePage,
			NiconicoPage = newVideoDTO.NiconicoPage,
			BilibiliPage = newVideoDTO.BilibiliPage,
			UploadDate = newVideoDTO.UploadDate,
			AuthorizedPerVideo = newVideoDTO.AuthorizedPerVideo,
			AdditionalRequirement = newVideoDTO.AdditionalRequirement,
			FinishedProductLink = newVideoDTO.FinishedProductLink
		};
		database.Videos.Add(newVideo);
		database.SaveChanges();
		return Ok(newVideoDTO);
	}

	[HttpPut, Authorize]
	[Route("{ID}/Thumbnail")]
	public IActionResult SetVideoThumbnail(long ID, IFormFile file)
	{
		string extension = "";
		switch (file.ContentType)
		{
			case MediaTypeNames.Image.Jpeg:
				extension = "jpg";
				break;
			case MediaTypeNames.Image.Png:
				extension = "png";
				break;
			case MediaTypeNames.Image.Webp:
				extension = "webp";
				break;
			case MediaTypeNames.Image.Tiff:
				extension = "tiff";
				break;
			default:
				return BadRequest();
		}
		long nn = file.Length;
		if (nn < 5 || nn > VideoID.MAX_THUMBNAIL_SIZE)
			return BadRequest();
		int n = (int)nn;
		if (!database.Videos.Any(x => x.ID == ID))
			return BadRequest();
		using Stream readStream = file.OpenReadStream();
		byte[] bytes = new byte[n];
		int ready = readStream.Read(bytes, 0, n);
		if (ready != n)
			throw new NTHException("video thumbnail stream guard");
		int updates = database.Videos.Where(z => z.ID == ID)
			.ExecuteUpdate(setter =>
				setter.SetProperty(x => x.ThumbnailType, extension)
					.SetProperty(x => x.Thumbnail, bytes));
		if (updates != 1)
			throw new NTHException("video executeupdate guard");
		return Ok("OK");
	}

	[HttpGet]
	[Route("{ID}/Thumbnail")]
	[ResponseCache(Duration = 86400 * 10)]
	public IActionResult GetVideoThumbnail(long ID)
	{
		var rkgk = database.Videos
			.Where(x => x.ID == ID)
			.Select(x => new { x.Title, x.Thumbnail, x.ThumbnailType })
			.FirstOrDefault();
		if (rkgk is null || rkgk.Thumbnail.Length < 5)
			return NotFound();
		string mime = "unknown";
		if ("avif".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Avif;
		else if ("bmp".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Bmp;
		else if ("gif".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Gif;
		else if ("ico".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Icon;
		else if ("jpg".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Jpeg;
		else if ("jpeg".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Jpeg;
		else if ("png".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Png;
		else if ("svg".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Svg;
		else if ("tiff".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Tiff;
		else if ("webp".Equals(rkgk.ThumbnailType, StringComparison.CurrentCultureIgnoreCase))
			mime = MediaTypeNames.Image.Webp;
		return File(rkgk.Thumbnail, mime, $"{rkgk.Title}.{rkgk.ThumbnailType}");
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
			x.NiconicoPage,
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
			x.NiconicoPage,
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
