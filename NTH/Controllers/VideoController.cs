using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.Video;

namespace NTH.Controllers;

[ApiController]
[Route("api/Video")]
public class VideoController(ILogger<VideoController> logger, PostgresContext database) : ControllerBase
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
            throw new Exception("video thumbnail stream guard");
        int updates = database.Videos.Where(z => z.ID == ID)
            .ExecuteUpdate(setter =>
                setter.SetProperty(x => x.ThumbnailType, extension)
                    .SetProperty(x => x.Thumbnail, bytes));
        if (updates != 1)
            throw new Exception("video executeupdate guard");
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
