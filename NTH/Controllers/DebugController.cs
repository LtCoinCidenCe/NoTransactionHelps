#if DEBUG
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;

namespace NTH.Controllers;

[ApiController]
[Route("api/Debug")]
public class DebugController : ControllerBase
{
	ILogger<DebugController> logger;
	ILogger<AuthorController> authorLogger;
	ILogger<UserController> userLogger;
	ILogger<VideoController> videoLogger;
	SQLiteContext database;
	UserService userService;
	SupplementaryService supplementaryService;
	AuthorService authorService;
	IConfiguration configuration;

	private AuthorController authorController;
	private UserController userController;
	private VideoController videoController;

	private Random random = new Random(894);

	public DebugController(
		ILogger<DebugController> dilogger,
		ILogger<AuthorController> diauthorLogger,
		ILogger<UserController> diuserLogger,
		ILogger<VideoController> divideoLogger,
		SQLiteContext didatabase,
		UserService diuserService,
		SupplementaryService disupplementaryService,
		AuthorService diauthorService,
		IConfiguration diConfiguration
	) : base()
	{
		logger = dilogger;
		authorLogger = diauthorLogger;
		userLogger = diuserLogger;
		videoLogger = divideoLogger;
		database = didatabase;
		userService = diuserService;
		supplementaryService = disupplementaryService;
		authorService = diauthorService;
		configuration = diConfiguration;

		authorController = new AuthorController(authorLogger, database, authorService, new RequestingUser());
		userController = new UserController(userLogger, database, userService, new RequestingUser());
		videoController = new VideoController(videoLogger, database, new RequestingUser());
	}

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
	[Route("[action]")]
	[Authorize]
	public IActionResult JWTPing()
	{
		var user = User;
		var httpcontext = HttpContext;
		string? identity = user.FindFirstValue(ClaimTypes.Anonymous);
		return Ok("In debug mode, auth");
	}

	[HttpGet]
	[Route("[action]")]
	[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
	public IActionResult CookiePing()
	{
		var user = User;
		return Ok("In debug mode, cookie");
	}

	private NonSensitiveUserDTO shifeng(NewUserDTO userDude)
	{
		List<ValidationResult> validationResults = [];
		bool goodValidated = Validator.TryValidateObject(userDude, new ValidationContext(userDude), validationResults, true);
		if (!goodValidated)
			throw new NTHException("Debug hardcoded users validation failed.");
		var newUserResult = (userController.CreateNewUser(userDude) as CreatedAtActionResult) ?? throw new NTHException("Debug hardcoded users creation failed");
		var newUserReturned = (newUserResult.Value as NonSensitiveUserDTO) ?? throw new NTHException("Debug hardcoded users return failed");
		return newUserReturned;
	}

	/// <summary>
	/// from rolls-royce engine blade pigtail
	/// </summary>
	/// <returns></returns>
	/// <exception cref="NTHException">should not happen</exception>
	[HttpDelete]
	[Route("[action]")]
	public async Task<ActionResult> InitializeProductionPigtail()
	{
		database.Database.EnsureDeleted();
		database.Database.EnsureCreated();
		// at least this is controller call, validation like this works
		NewUserDTO newUser = new() { Username = "Genesis", Displayname = "Genesis begins", Password = "apetonxin9320" };
		List<ValidationResult> validationResults = [];
		bool goodValidated = Validator.TryValidateObject(newUser, new ValidationContext(newUser), validationResults, true);
		if (!goodValidated)
			throw new NTHException("pigtail generation failure. First User Validation.");

		// generic ActionResult<T>.Value fools people
		var newUserResult = (userController.CreateNewUser(newUser) as CreatedAtActionResult) ?? throw new NTHException("pigtail generation failure, First user creation");
		var newUserReturned = (newUserResult.Value as NonSensitiveUserDTO) ?? throw new NTHException("pigtail generation failure, First user return");
		userController.SetUserRole(newUserReturned.ID, UserRoleDTO.God);
		return Ok("OK");
	}

	[HttpDelete]
	[Route("[action]")]
	public async Task<IActionResult> InitializeDatabase()
	{
		logger.Log(LogLevel.Warning, "Database Reinitializing");

		byte[] anIconFile;
		try
		{
			anIconFile = System.IO.File.ReadAllBytes("./鱼卡日yu.png");
		}
		catch (FileNotFoundException)
		{
			throw new NTHException("Debug Initialization 鱼卡日 file not found. Check if this program is running debug.");
		}

		await InitializeProductionPigtail();

		supplementaryService.GenerateSupplementaryDefinition();

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

		userController.ControllerContext = ControllerContext;
		authorController.ControllerContext = ControllerContext;

		string samplePassword = "kissa123";
		List<NewUserDTO> newUsersDTO = [
			new() { Username = "FirstUser", Displayname = "The First Emperor", Password = "someDefault" },
			new() { Username = "krk", Displayname = "Kimi Räikkönen", Password = "McLaren" },
			new() { Username = "string", Displayname = "testUser", Password = "string" },
			new() { Username = "star", Displayname = "Solar", Password = "texas" },
			new() { Username = "ayjyou", Displayname = "Yajyou", Password = "114514" },
			new() { Username = "nononononofs", Displayname = "sfononono", Password = "perisrtow" },
			new() { Username = "anguraea", Displayname = "Angular", Password = "whatisthat?" },
			new() { Username = "oofran", Displayname = "Francais", Password = "bonne1846" },
			new() { Username = "pstrag", Displayname = "Patient Strategizer", Password = "someApexmeme" },
			new() { Username = "heathrow", Displayname = "London Heathrow", Password = "someApexmeme" },
			new() { Username = "apexTan", Displayname = "apexTan", Password = "someApexmeme" },
			new() { Username = "calmcalcu", Displayname = "Calm Calculator", Password = "someApexmeme" },
			new() { Username = "upperstar", Displayname = "Upper Star", Password = "someApexmeme" },
			new() { Username = "dstriker", Displayname = "Dual Strike", Password = "someApexmeme" },
			new() { Username = "hoshik", Displayname = "Hoshi no Kaabi", Password = "someApexmeme" },
			new() { Username = "kirby", Displayname = "WellIamKirby", Password = "someApexmeme" },
			new() { Username = "fairchild", Displayname = "Fairchild", Password = "someApexmeme" },
			new() { Displayname = "BusinessInside", Username = "business", Password = samplePassword }
		];
		var newUsers = newUsersDTO.Select(shifeng).ToList();

		var firstUser = newUsers.Single(x => x.Username == "FirstUser");
		var testUser = newUsers.Single(x => x.Username == "string");
		var starUser = newUsers.Single(x => x.Username == "star");
		var Angular = newUsers.Single(x => x.Username == "anguraea");
		var franc = newUsers.Single(x => x.Username == "oofran");
		var PatientStrategizer = newUsers.Single(x => x.Username == "pstrag");
		var LondonHeathrow = newUsers.Single(x => x.Username == "heathrow");
		var businessman = newUsers.Single(x => x.Username == "business");
		userController.SetUserRole(firstUser.ID, UserRoleDTO.SystemAdministrator | UserRoleDTO.Translator);
		userController.SetUserRole(testUser.ID, UserRoleDTO.SystemAdministrator);
		userController.SetUserRole(starUser.ID, UserRoleDTO.SystemAdministrator);
		userController.SetUserRole(Angular.ID, UserRoleDTO.Translator | UserRoleDTO.Scriptor);
		userController.SetUserRole(franc.ID, UserRoleDTO.Scriptor);
		userController.SetUserRole(PatientStrategizer.ID, UserRoleDTO.Translator);
		userController.SetUserRole(LondonHeathrow.ID, UserRoleDTO.Translator);

		userController = new UserController(userLogger, database, userService, new RequestingUser() { UserID = starUser.ID, UserRole = UserRoleDTO.SystemAdministrator });

		for (int i = 1; i <= 6; i++)
		{
			userController.SetUserIcon(i, new FormFile(new MemoryStream(anIconFile), 0, anIconFile.Length, "icon", "鱼卡日yu.png"));
		}

		userController = new UserController(userLogger, database, userService, new RequestingUser() { UserID = businessman.ID, UserRole = UserRoleDTO.User });
		authorController = new AuthorController(authorLogger, database, authorService, new RequestingUser() { UserID = businessman.ID, UserRole = UserRoleDTO.User });
		var businessRoleResult = userController.SetUserRole(businessman.ID, UserRoleDTO.Translator | UserRoleDTO.Scriptor);

		List<NewAuthorDTO> moreAuthors = [
			new NewAuthorDTO() { Name = "kflat", TwitterHomePage = "https://x.com/kflat_aasa" },
			new NewAuthorDTO() { Name = "cyderl", },
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
		moreAuthors.Select(authorController.CreateNewAuthor);

		for (int i = 0; i < moreAuthors.Count * 2; i++)
		{
			authorController.SetContact(authorID: random.Next(moreAuthors.Count) + 1, userID: random.Next(newUsers.Count) + 1);
		}

		videoController = new VideoController(videoLogger, database, new RequestingUser() { UserID = 2, UserRole = UserRoleDTO.SystemAdministrator });
		videoController.CreateNewVideo(new NewVideoDTO { AuthorID = 1, Title = "过♂年", BilibiliPage = "https://www.bilibili.com/video/BV1Qs411X7QR" });

		logger.Log(LogLevel.Warning, "Database debug initialized.");
		return Ok("Initialized");
	}
}
#endif
