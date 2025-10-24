#if DEBUG
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.User;
using NTH.Services;

namespace NTH.Controllers;

[ApiController]
[Route("Debug")]
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

    [HttpDelete]
    public void InitializeDatabase()
    {
        logger.Log(LogLevel.Warning, "Database Reinitializing");
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();

        supplementaryService.GenerateSupplementaryDefinition();

        var firstUser = userService.CreateNewUser(new NewUser
        {
            Username = "FirstUser",
            Displayname = "The First Emperor",
            Password = "someDefault"
        });

        logger.Log(LogLevel.Warning, JsonSerializer.Serialize(firstUser));
    }

    [HttpGet]
    [Route("GetUsers")]
    public List<UserID> GetUsers()
    {
        return database.Users
            .AsSplitQuery()
            .Include(x => x.DisplaynameHistory)
            .Include(x => x.UserRoleHistory)
            .AsNoTracking()
            .ToList();
    }
}
#endif
