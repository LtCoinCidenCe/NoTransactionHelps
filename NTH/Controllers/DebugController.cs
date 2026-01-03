#if DEBUG
using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.Author;
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
ILogger<AuthorController> authorLogger,
ILogger<UserController> userLogger,
PostgresContext database,
UserService userService,
SupplementaryService supplementaryService,
AuthorService authorService) : ControllerBase
{
    private AuthorController authorController = new AuthorController(authorLogger, database, authorService);
    private UserController userController = new UserController(userLogger, database, userService);

    /// <summary>
    /// Debug/test mode indicator
    /// this should not be detected in production mode
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("ping")]
    public IActionResult Ping()
    {
        string somehttp = "https://www.isodfe.com/video/234?ist=true&d=6#title";
        Uri uri = new Uri(somehttp);
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
    public IActionResult InitializeDatabase()
    {
        logger.Log(LogLevel.Warning, "Database Reinitializing");
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();

        userController.ControllerContext = ControllerContext;
        authorController.ControllerContext = ControllerContext;

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
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "ayjyou",
            Displayname = "Yajyou",
            Password = "114514"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "nononononofs",
            Displayname = "sfononono",
            Password = "perisrtow"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "anguraea",
            Displayname = "Angular",
            Password = "whatisthat?"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "oofran",
            Displayname = "Francais",
            Password = "bonne1846"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "pstrag",
            Displayname = "Patient Strategizer",
            Password = "someApexmeme"
        });
        userService.CreateNewUser(new NewUserDTO
        {
            Username = "heathrow",
            Displayname = "London Heathrow",
            Password = "someApexmeme"
        });

        // size = 6, [0-5]
        List<DateTimeOffset> times = [
            new DateTimeOffset(2024, 5, 6, 12, 25, 30, TimeSpan.Zero),
            new DateTimeOffset(2025, 2, 9, 7, 4, 21, TimeSpan.Zero),
            new DateTimeOffset(2024, 7, 9, 20, 43, 44, TimeSpan.Zero),
            new DateTimeOffset(2024, 12, 25, 12, 25, 3, TimeSpan.Zero),
            new DateTimeOffset(2025, 9, 4, 23, 55, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 11, 3, 6, 12, 9, TimeSpan.Zero)
        ];
        times.Sort();


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
        List<NewAuthorDTO> moreAuthors = [
            new NewAuthorDTO() { Name = "harujiko", },
            new NewAuthorDTO() { Name = "suyako", AllVideoAuthorized = true },
            new NewAuthorDTO() { Name = "awawa", AuthorizedPerVideo = true, TensaiRequirement = "提供作者主页链接" },
            new NewAuthorDTO() { Name = "tatin", },
            new NewAuthorDTO() { Name = "しろめで", },
            new NewAuthorDTO() { Name = "descend", },
            new NewAuthorDTO() { Name = "ねねこ", },
            new NewAuthorDTO() { Name = "w-mine", },
            new NewAuthorDTO() { Name = "saphire", AuthorizedPerVideo = true, TensaiRequirement = "提供作者主页链接" },
            new NewAuthorDTO() { Name = "NpU", },
            new NewAuthorDTO() { Name = "KLM", AllVideoAuthorized = true },
            new NewAuthorDTO() { Name = "ANA", AuthorizedPerVideo = true, TensaiRequirement = "提供作者主页链接" },
            new NewAuthorDTO() { Name = "JAL", },
        ];
        var creatingAuthors = moreAuthors.Select(x => authorController.CreateNewAuthor(x)).ToList();

        businessman.Contact.Add(new WorkContact() { Author = oneAuthor });
        businessman.Contact.Add(new WorkContact() { Author = twoAuthor });
        var firstVideo = new VideoID()
        {
            Title = "过♂年",
            BilibiliPage = "https://www.bilibili.com/video/BV1Qs411X7QR",
        };
        oneAuthor.Videos.Add(firstVideo);

        if (firstUser is null)
            throw new Exception("firstUser null guard");
        userService.SetUserRole(firstUser.ID, UserRoleDTO.Translator);
        firstVideo.Works.Add(new WorkID
        {
            UserID = firstUser.ID,
            FinishingDate = DateTimeOffset.UtcNow,
        });
        firstVideo.StatusTranslation = WorkStatus.Assigned;
        database.SaveChanges();

        logger.Log(LogLevel.Warning, "Database debug initialized.");
        return Ok("Initialized");
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
        return Ok("OK");
    }
}
#endif
