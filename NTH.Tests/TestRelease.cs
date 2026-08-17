#if !DEBUG
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTH.Tests;

[TestClass]
public sealed class TestRelease
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
	public async Task ReleaseCommitPrepared()
	{
		// This will be modified in the pipeline
		// to ensure that the commit hash is not the default value.
#pragma warning disable MSTEST0025, MSTEST0032
		Assert.AreNotEqual("Not a release commit", PipelineGitCommit.CurrentCommitHash);
#pragma warning restore MSTEST0025, MSTEST0032
		var response = await client.GetAsync("api/Ping/Commit");
		response.EnsureSuccessStatusCode();
		var fromWebAPI = await response.Content.ReadAsStringAsync();
		Assert.AreNotEqual("Not a release commit", fromWebAPI);

		var noDebug = await client.GetAsync("api/Debug/ping");
		Assert.AreNotEqual("In debug mode", await noDebug.Content.ReadAsStringAsync());
	}
}
#endif
