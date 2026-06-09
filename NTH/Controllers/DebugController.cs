#if DEBUG
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
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
public class DebugController : ControllerBase
{
	ILogger<DebugController> logger;
	ILogger<AuthorController> authorLogger;
	ILogger<UserController> userLogger;
	PostgresContext database;
	UserService userService;
	SupplementaryService supplementaryService;
	AuthorService authorService;

	private AuthorController authorController;
	private UserController userController;
	private static HttpClient httpClient = new HttpClient();

	public DebugController(
		ILogger<DebugController> dilogger,
		ILogger<AuthorController> diauthorLogger,
		ILogger<UserController> diuserLogger,
		PostgresContext didatabase,
		UserService diuserService,
		SupplementaryService disupplementaryService,
		AuthorService diauthorService
	) : base()
	{
		logger = dilogger;
		authorLogger = diauthorLogger;
		userLogger = diuserLogger;
		database = didatabase;
		userService = diuserService;
		supplementaryService = disupplementaryService;
		authorService = diauthorService;

		authorController = new AuthorController(authorLogger, database, authorService);
		userController = new UserController(userLogger, database, userService, new RequestingUser());
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

	[HttpGet]
	[Route("tempReturn")]
	public IActionResult TempReturn()
	{
		var urlCreateUser = Url.Action("CreateNewUser", "User");
		var urlSetUserRole = Url.Action(nameof(UserController.SetUserRole), "User", new { ID = 9 });
		return Ok(urlSetUserRole);
	}

	[HttpGet]
	[Route("httpAuth")]
	public async Task<IActionResult> HttpGo()
	{
		var asyncCall = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}/api/Login", new UserLoginDTO() { Username = "star", Password = "texas" });
		var jwt = await asyncCall.Content.ReadAsStringAsync();
		if (string.IsNullOrEmpty(jwt))
			throw new NTHException("httpAuth jwt is not received");
		return Ok("OK");
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
			throw new NTHException("pigtail generation failure");

		// generic ActionResult<T>.Value fools people
		var newUserResult = (userController.CreateNewUser(newUser) as CreatedAtActionResult) ?? throw new NTHException("pigtail generation failure");
		var newUserReturned = (newUserResult.Value as NonSensitiveUserDTO) ?? throw new NTHException("pigtail generation failure");
		userController.SetUserRole(newUserReturned.ID, UserRoleDTO.SuperAdministrator);
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
			anIconFile = System.IO.File.ReadAllBytes("../鱼卡日yu.png");
		}
		catch (FileNotFoundException)
		{
			throw new NTHException("Debug Initialization 鱼卡日 file not found. Check if this program is running debug.");
		}

		await InitializeProductionPigtail();

		supplementaryService.GenerateSupplementaryDefinition();

		userController.ControllerContext = ControllerContext;
		authorController.ControllerContext = ControllerContext;

		var jwtForFirstUserCreation = getJwtByUser(new UserLoginDTO { Username = "Genesis", Password = "apetonxin9320" }).Result;
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtForFirstUserCreation);

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
		];
		var urlCreateUser = Url.Action(nameof(UserController.CreateNewUser), "User");
		var shifeng = async (NewUserDTO dude) =>
		{
			var httpResult = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlCreateUser}", dude);
			var resultObject = await httpResult.Content.ReadFromJsonAsync<NonSensitiveUserDTO>();
			if (resultObject is null)
				throw new NTHException("Debug User Creation Error");
			return resultObject;
		};
		var newUsers = newUsersDTO.Select(x =>
		{
			return shifeng(x).Result;
		}).ToList();


		var firstUser = newUsers.Single(x => x.Username == "FirstUser");
		var testUser = newUsers.Single(x => x.Username == "string");
		var starUser = newUsers.Single(x => x.Username == "star");
		var Angular = newUsers.Single(x => x.Username == "anguraea");
		var franc = newUsers.Single(x => x.Username == "oofran");
		var PatientStrategizer = newUsers.Single(x => x.Username == "pstrag");
		var LondonHeathrow = newUsers.Single(x => x.Username == "heathrow");

		var jwtForRole = getJwtByUser(new UserLoginDTO { Username = "krk", Password = "McLaren" }).Result;
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtForRole);

		async Task<UserRoleHistory> fengdi(long ID, UserRoleDTO newRole)
		{
			var urlSetUserRole = Url.Action(nameof(UserController.SetUserRole), "User", new { ID });
			var httpResult = await httpClient.PutAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlSetUserRole}", newRole);
			var resultObject = await httpResult.Content.ReadFromJsonAsync<UserRoleHistory>() ?? throw new NTHException("Debug Set User Role Error");
			return resultObject;
		}

		var a2 = fengdi(firstUser.ID, UserRoleDTO.SystemAdministrator | UserRoleDTO.Translator).Result;
		a2 = fengdi(testUser.ID, UserRoleDTO.SystemAdministrator).Result;
		a2 = fengdi(starUser.ID, UserRoleDTO.SystemAdministrator).Result;
		a2 = fengdi(Angular.ID, UserRoleDTO.Translator | UserRoleDTO.Scriptor).Result;
		a2 = fengdi(franc.ID, UserRoleDTO.Scriptor).Result;
		a2 = fengdi(PatientStrategizer.ID, UserRoleDTO.Translator).Result;
		a2 = fengdi(LondonHeathrow.ID, UserRoleDTO.Translator).Result;

		var setProfileIcons = async () =>
		{
			var jwt = await getJwtByUser(new UserLoginDTO { Username = "star", Password = "texas" });
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			using var content = new MultipartFormDataContent();
			content.Add(new ByteArrayContent(anIconFile), "icon", "鱼卡日yu.png");
			List<Task<HttpResponseMessage>> iconCalls = new();
			for (int i = 1; i <= 6; i++)
			{
				var URLi = $"{Request.Scheme}://{Request.Host}/api/User/{i}/Icon";
				var iconCall = await httpClient.PutAsync(URLi, content);
				// iconCalls.Add(iconCall);
			}
			// await Task.WhenAll(iconCalls);
		};

		await setProfileIcons();

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

		NewUserDTO businessDTO = new()
		{
			Displayname = "BusinessInside",
			Username = "business",
			Password = samplePassword
		};
		var businessman = await shifeng(businessDTO);
		string jwtForBusinessman = await getJwtByUser(new UserLoginDTO { Username = businessman.Username, Password = samplePassword });
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtForBusinessman);
		var urlSetBusinessTitleWords = Url.Action(nameof(UserController.SetTitleWords), "User", new { businessman.ID });
		var titleWordResult = await httpClient.PutAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlSetBusinessTitleWords}", "The new king.");
		var businessRoleResult = await fengdi(businessman.ID, UserRoleDTO.Translator | UserRoleDTO.Scriptor);

		var urlCreateAuthor = Url.Action(nameof(AuthorController.CreateNewAuthor), "Author");
		var oneAuthor = new NewAuthorDTO()
		{
			Name = "kflat",
			TwitterHomePage = "https://x.com/kflat_aasa",
		};
		var oneAuthorCreation = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlCreateAuthor}", oneAuthor);
		var twoAuthor = new NewAuthorDTO()
		{
			Name = "cyderl",
		};
		var twoAuthorCreation = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlCreateAuthor}", twoAuthor);
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
		var creatingAuthors = await Task.WhenAll(moreAuthors.Select(x => httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlCreateAuthor}", x)));

		var urlCreateVideo = Url.Action(nameof(VideoController.CreateNewVideo), "Video");
		var firstVideo = new NewVideoDTO
		{
			AuthorID = 1,
			Title = "过♂年",
			BilibiliPage = "https://www.bilibili.com/video/BV1Qs411X7QR",
		};
		var oneVideoCreation = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}{urlCreateVideo}", firstVideo);

		// firstVideo.Works.Add(new WorkID
		// {
		// 	UserID = firstUser.ID,
		// 	FinishingDate = DateTimeOffset.UtcNow,
		// });
		// firstVideo.StatusTranslation = WorkStatus.Assigned;
		// database.SaveChanges();

		logger.Log(LogLevel.Warning, "Database debug initialized.");
		return Ok("Initialized");
	}

	private async Task<string> getJwtByUser(UserLoginDTO loginUser)
	{
		var authCall = await httpClient.PostAsJsonAsync($"{Request.Scheme}://{Request.Host}/api/Login", loginUser);
		var jwt = await authCall.Content.ReadAsStringAsync();
		if (string.IsNullOrEmpty(jwt))
			throw new NTHException("httpAuth jwt is not received");
		return jwt;
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
		//BackgroundJob.Schedule(
		//    () => SchedulingTasks.ThrowException(),
		//    TimeSpan.FromSeconds(15)
		//);
		return Ok("OK");
	}
}
#endif
