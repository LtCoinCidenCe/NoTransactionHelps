#if DEBUG
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTH.Controllers;
using NTH.DBContext;
using NTH.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NTH.Tests;

[TestClass]
public sealed class TestDebug
{
	private static readonly WebApplicationFactory<Program> _factory = new();
	private static readonly HttpClient client = _factory.CreateClient();
	private static string bossJWT = string.Empty;

	// MSTest automatically sets the TestContext property before each test runs.
	// MSTest.Analyzers includes a diagnostic suppressor that removes CS8618
	// (non-nullable property uninitialized) for this property.
	public TestContext TestContext { get; set; }

	[AssemblyInitialize]
	public static void AssemblyInit(TestContext context)
	{
		// This method is called once for the test assembly, before any tests are run.
	}

	[AssemblyCleanup]
	public static void AssemblyCleanup()
	{
		// This method is called once for the test assembly, after all tests are run.
	}

	[ClassInitialize]
	public static async Task ClassInit(TestContext context)
	{
		// This method is called once for the test class, before any tests of the class are run.
		var response = await client.DeleteAsync($"api/Debug/{nameof(DebugController.InitializeDatabase)}");
		response.EnsureSuccessStatusCode();

		var bossCall = await client.PostAsJsonAsync("api/Login", new UserLoginDTO { Username = "Genesis", Password = "apetonxin9320" });
		bossCall.EnsureSuccessStatusCode();
		bossJWT = await bossCall.Content.ReadAsStringAsync();
	}

	[ClassCleanup]
	public static async Task ClassCleanup()
	{
		// This method is called once for the test class, after all tests of the class are run.
	}

	[TestInitialize]
	public void TestInit()
	{
		// This method is called before each test method.
	}

	[TestCleanup]
	public void TestCleanup()
	{
		// This method is called after each test method.
	}

	[TestMethod]
	public async Task BasicPingTest()
	{
		var response = await client.GetAsync("api/ping");
		response.EnsureSuccessStatusCode();
		Assert.AreEqual(200, (int)response.StatusCode);
	}

	[TestMethod]
	public async Task BasicTableDataExists()
	{
		SQLiteContext dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<SQLiteContext>();
		Assert.IsTrue(dbContext.Users.Any());
		Assert.IsNotNull(dbContext.Users.FirstOrDefault(x => x.Username == "apexTan"));
	}

	[TestMethod]
	public async Task DebugAvailable()
	{
		var isDebug = await client.GetAsync("api/Debug/ping");
		Assert.AreEqual("In debug mode", await isDebug.Content.ReadAsStringAsync());
	}

	[TestMethod]
	public async Task DLPSystemTest()
	{
		List<int> nicoAuthorsID = [
			118691209, // an author くうい
			90869956, // an author えよくれ
			92143777, // an author Hetzer
		];

		var jwtcall = await client.PostAsJsonAsync("api/Login", new UserLoginDTO { Username = "string", Password = "string" });
		jwtcall.EnsureSuccessStatusCode();
		var jwt = await jwtcall.Content.ReadAsStringAsync();

		foreach (var nicoA in nicoAuthorsID)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "api/Author/dlp");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			request.Content = JsonContent.Create(nicoA);
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
		await Task.Delay(3000);
		Assert.AreEqual(0, YtdlpInstanceService.TaskStation.CurrentCount);

		for (int i = 0; i < nicoAuthorsID.Count; i++)
		{
			await YtdlpInstanceService.TaskStation.WaitAsync(TestContext.CancellationToken);
			if (YtdlpInstanceService.taskCompletionCount == nicoAuthorsID.Count)
				break;
			YtdlpInstanceService.TaskStation.Release();
			await Task.Delay(2000);
		}
		SQLiteContext dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<SQLiteContext>();
		var author = await dbContext.Authors.Include(x => x.Videos).AsNoTracking().FirstOrDefaultAsync(x => x.Name == "くうい");
		Assert.IsNotNull(author, "Targeted author is not documented");
		Assert.IsGreaterThan(3, author.Videos.Count, "Targeted videos are not documented enough");
	}

	[TestMethod]
	public async Task UserInvitationSystemTest()
	{
		var jwtcall = await client.PostAsJsonAsync("api/Login", new UserLoginDTO { Username = "star", Password = "texas" });
		jwtcall.EnsureSuccessStatusCode();
		var jwt = await jwtcall.Content.ReadAsStringAsync();

		var request = new HttpRequestMessage(HttpMethod.Post, "api/Login/InvitationLink");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
		request.Content = new StringContent("");
		var response = await client.SendAsync(request);
		response.EnsureSuccessStatusCode();
		for (int i = 0; i < 4; i++)
		{
			request = new HttpRequestMessage(HttpMethod.Post, "api/Login/InvitationLink");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
			request.Content = new StringContent("");
			response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
		}
		var ljnk = await response.Content.ReadAsStringAsync();

		var theURI = new Uri(ljnk);
		var it = theURI.Query.IndexOf("token");
		var token = theURI.Query.Substring(it + 6).Split('&')[0];
		var j = theURI.Query.IndexOf("ID");
		var ID = long.Parse(theURI.Query.Substring(j + 3, 6).Split('&')[0]);

		request = new HttpRequestMessage(HttpMethod.Post, "api/Login/InvitedAccountCreation");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
		var dto = new AccountCreationDTO()
		{
			ID = ID,
			Token = token,
			NewUser = new NewUserDTO()
			{
				Username = "TGU",
				Displayname = "Test generated user",
				Password = "testerPassword"
			}
		};
		request.Content = JsonContent.Create(dto);
		response = await client.SendAsync(request);
		response.EnsureSuccessStatusCode();

		SQLiteContext dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<SQLiteContext>();
		var invitationDB = dbContext.UserInvitationLinks.AsNoTracking().FirstOrDefault(x => x.ID == ID);
		Assert.IsNotNull(invitationDB);
		Assert.IsNotNull(invitationDB.CreatedUser);
		var createdUser = dbContext.Users.AsNoTracking().FirstOrDefault(x => x.ID == invitationDB.CreatedUser);
		Assert.IsNotNull(createdUser);
		Assert.AreEqual("TGU", createdUser.Username);
		Assert.AreEqual("Test generated user", createdUser.Displayname);
	}
}
#endif
