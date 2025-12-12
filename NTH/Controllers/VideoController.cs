using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            .Select(x => x) // TODO: project the query to make it easier and quicker.
            .ToList();
        return Ok(validVideos);
    }
}
