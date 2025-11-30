#if DEBUG
using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.Author;
using NTH.Models.User;
using NTH.Models.Video;
using NTH.Models.Work;
using NTH.Scheduling;
using NTH.Services;
using NTH.Utilities;

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
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "string",
            Displayname = "testUser",
            Password = "string"
        });


        string samplePassword = "kissa123";
        string? salt = null;
        byte[] hashed = PasswordHasher.GetHashedPassword(samplePassword, ref salt);
        if (salt is null)
            throw new PasswordHasherException("salt is not received");
        var businessman = new UserID
        {
            Username = "business",
            Displayname = "BusinessInside",
            Password = hashed,
            PassSalt = salt,
        };
        database.Users.Add(businessman);
        database.SaveChanges();
        businessman.DisplaynameHistory.Add(new DisplaynameHistory
        {
            UserID = businessman.ID,
            Displayname = businessman.Displayname,
            CreationDate = businessman.CreationDate,
        });
        userService.SetTitleWords(businessman.ID, "The new king.");
        userService.SetUserRole(businessman.ID, UserRoleDTO.Translator | UserRoleDTO.Scriptor);
        var oneAuthor = new AuthorID()
        {
            Name = "kflat",
            TwitterHomePage = "https://x.com/kflat_aasa",
        };
        var twoAuthor = new AuthorID()
        {
            Name = "cyderl",
        };
        businessman.Contact.Add(new WorkContact()
        {
            Author = oneAuthor
        });
        businessman.Contact.Add(new WorkContact()
        {
            Author = twoAuthor
        });
        var firstVideo = new VideoID()
        {
            Title = "过♂年",
            BilibiliPage = "https://www.bilibili.com/video/BV1Qs411X7QR",
        };
        oneAuthor.Videos.Add(firstVideo);

        if (firstUser is null)
            throw new Exception("firstUser null guard");
        userService.SetUserRole(firstUser.ID, UserRoleDTO.Translator);
        firstVideo.WorkTranslation.Add(new WorkTranslation
        {
            UserID = firstUser.ID,
            ChangeDate = DateTimeOffset.UtcNow,
        });
        firstVideo.StatusTranslation = WorkStatus.Assigned;
        database.SaveChanges();

        logger.Log(LogLevel.Warning, "Database debug initialized.");
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
            .Include(x => x.WorkTranslations)
            .ThenInclude(x => x.Video)
            .ToList();
        return users;
    }

    [HttpGet]
    [Route("UserIcon/{ID}")]
    public ActionResult GetIconbyID(long ID)
    {
        var row = database.UserIconHistories.AsNoTracking().FirstOrDefault(x => x.ID == ID);
        if (row is null)
            return NotFound();
        return File(row.Icon, "image/png");
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
