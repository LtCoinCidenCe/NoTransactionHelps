using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTH.Controllers;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTH.Tests;

[TestClass]
public sealed class Test1
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
}
