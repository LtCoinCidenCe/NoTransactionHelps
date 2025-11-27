using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;

namespace NTH.Controllers;

[ApiController]
[Route("api/Video")]
public class VideoController(ILogger<VideoController> logger, PostgresContext database) : ControllerBase
{
    
}