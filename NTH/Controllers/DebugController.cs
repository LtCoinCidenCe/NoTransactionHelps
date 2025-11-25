#if DEBUG
using System.Security.Claims;
using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.User;
using NTH.Scheduling;
using NTH.Services;

namespace NTH.Controllers;

[ApiController]
[Route("api/Debug")]
public partial class DebugController(
ILogger<DebugController> logger,
PostgresContext database,
UserService userService,
SupplementaryService supplementaryService) : ControllerBase
{
    /// <summary>
    /// Debug/test mode indicator
    /// this should not be detected in production mode
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("ping")]
    public IActionResult Ping()
    {
        return Ok("In debug mode");
    }

    [HttpGet]
    [Route("authping")]
    [Authorize]
    public IActionResult AuthPing()
    {
        var user = User;
        var httpcontext = HttpContext;
        string? identity = user.FindFirstValue(ClaimTypes.Anonymous);
        return Ok("In debug mode, auth");
    }

    [HttpDelete]
    public void InitializeDatabase()
    {
        logger.Log(LogLevel.Warning, "Database Reinitializing");
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();

        supplementaryService.GenerateSupplementaryDefinition();

        var firstUser = userService.CreateNewUser(new NewUserDTO
        {
            Username = "FirstUser",
            Displayname = "The First Emperor",
            Password = "someDefault"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "krk",
            Displayname = "Kimi Räikkönen",
            Password = "McLaren"
        });

        logger.Log(LogLevel.Warning, JsonSerializer.Serialize(firstUser));
    }

    [HttpGet]
    [Route("GetUsers")]
    public List<UserID> GetUsers()
    {
        return database.Users
            // .AsSplitQuery()
            .AsSingleQuery()
            .AsNoTracking()
            .Include(x => x.DisplaynameHistory)
            .Include(x => x.UserRoleHistory)
            .Include(x => x.Contact)
            .ThenInclude(contact => contact.Author)
            .Include(x => x.WorkTranslations)
            .ThenInclude(x => x.Video)
            .ToList();
    }

    [HttpGet]
    [Route("fireException")]
    public IActionResult FireException()
    {
        BackgroundJob.Schedule(
            () => SchedulingTasks.ThrowException(),
            TimeSpan.FromSeconds(15)
        );
        return Ok();
    }
}
#endif
