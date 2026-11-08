using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;
using NTH.Middlewares;

namespace NTH.Controllers;

[ApiController]
[Authorize]
[Route("api/Treasure")]
#pragma warning disable CS9113 // 参数未读。
public class TreasureController(ILogger<TreasureController> logger,
	SQLiteContext database,
	[FromServices] RequestingUser requestingUser) : ControllerBase
#pragma warning restore CS9113 // 参数未读。
{
	private static readonly Regex fileExtCheck = new Regex(@"^\.[A-Za-z0-9]{2,8}$", RegexOptions.Compiled);

	[HttpPost]
	[Route("File")]
	public async Task<IActionResult> UploadFile(IFormFile icon)
	{
		if (icon.Length < 5 || icon.Length > 1_000_000_000)
			return BadRequest("Size not good");
		string fileExtension = Path.GetExtension(icon.FileName);
		if (!fileExtCheck.IsMatch(fileExtension))
			return BadRequest("Extension not good");
		fileExtension = fileExtension[1..];
		using Stream readStream = icon.OpenReadStream();
		var savedFolder = Path.Join(TreasureCookieAssetController.TreasurePath, fileExtension);
		Directory.CreateDirectory(savedFolder);
		var id = Guid.CreateVersion7();
		var savedPath = $"{savedFolder}/{id}.{fileExtension}";
		using var ws = System.IO.File.OpenWrite(savedPath);
		await readStream.CopyToAsync(ws);
		await readStream.FlushAsync();
		await ws.FlushAsync();
		return Ok("OK");
	}
}

/// <summary>
/// 希望所有的资源作为静态文件从后缀名和GUID来获得
/// cookie验证
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("api/Treasure")]
public class TreasureCookieAssetController : ControllerBase
{
	[HttpGet]
	[Route("File/{ext}/{FileID}")]
	[ResponseCache(Duration = 60 * 60 * 24 * 30)]
	public IActionResult GetFileByID([FromRoute] string ext, [FromRoute] Guid FileID)
	{
		if (FileID == default)
			return NotFound();
		var potentialFile = Path.Join(TreasurePath, ext, FileID.ToString() + '.' + ext);
		FileStream fs;
		try { fs = System.IO.File.OpenRead(potentialFile); }
		catch (Exception) { return NotFound(); }
		return File(fs, "application/octet-stream", $"{FileID}.{ext}");
	}
	public static string TreasurePath = null!;
}
