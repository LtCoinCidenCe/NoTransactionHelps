#if DEBUG
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTH.Controllers;
using NTH.DBContext;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTH.Tests;

[TestClass]
public sealed class TestDebug
{
	private static readonly WebApplicationFactory<Program> _factory = new();
	private static readonly HttpClient client = _factory.CreateClient();

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
	}

	[ClassCleanup]
	public static void ClassCleanup()
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
}
#endif
