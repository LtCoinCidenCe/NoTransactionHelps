using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;
using SixLabors.ImageSharp;

namespace NTH.Controllers;

[ApiController]
[Authorize]
[Route("api/User")]
#pragma warning disable CS9113 // 参数未读。
public class UserController(ILogger<UserController> logger,
	SQLiteContext database,
	UserService userService,
	[FromServices] RequestingUser requestingUser) : ControllerBase
#pragma warning restore CS9113 // 参数未读。
{
	[HttpGet]
	public ICollection GetUsers()
	{
		var users = database.Users
			.AsNoTracking()
			.Where(x => !x.IsDeleted)
			.Include(x => x.Contact)
			.ThenInclude(contact => contact.Author)
			.ToList();
		users.ForEach(x =>
		{
			// non-public data
			x.PassSalt = "";
			x.Password = [];
			x.PasswordChangeDate = DateTimeOffset.MinValue;
		});
		return users;
	}

	[HttpGet]
	[Route("{ID}")]
	public ActionResult<NonSensitiveUserDTO> GetUser(string ID)
	{
		var user = userService.GetUserByID(ID);
		if (user is null)
		{
			return NotFound();
		}
		return Ok(NonSensitiveUserDTO.FromDBModel(user));
	}

	[HttpPost]
	public ActionResult CreateNewUser(NewUserDTO newUser)
	{
		UserID? newUserID = userService.CreateNewUser(newUser);
		if (newUserID is null)
			return BadRequest();
		return CreatedAtAction(nameof(CreateNewUser), NonSensitiveUserDTO.FromDBModel(newUserID));
	}

	[HttpPut]
	[Route("{ID}/DisplayName")]
	public IActionResult SetDisplayName(long ID, [Length(2, 30)][FromBody] string newDisplayName)
	{
		if (requestingUser.UserID != ID)
			if ((requestingUser.UserRole & UserRoleDTO.SystemAdministrator) != UserRoleDTO.SystemAdministrator)
				return Unauthorized();

		DisplaynameHistory? result = userService.SetDisplayName(ID, newDisplayName, null);
		if (result is null)
			return NotFound();
		return Ok(result);
	}

	[HttpPut]
	[Route("{ID}/TitleWords")]
	public IActionResult SetTitleWords(long ID, [MaxLength(250)][FromBody] string newTitleWords)
	{
		if (requestingUser.UserID != ID)
			if ((requestingUser.UserRole & UserRoleDTO.SystemAdministrator) != UserRoleDTO.SystemAdministrator)
				return Unauthorized();

		int rows = userService.SetTitleWords(ID, newTitleWords);
		if (rows == 1)
			return Ok("OK");
		else if (rows == 0)
			return NotFound();
		else
			throw new NTHException("SetTitleWords updated multiple rows");
	}

	[HttpPut]
	[Route("{ID}/Password")]
	public IActionResult SetPassword(long ID, [MinLength(5)][FromBody] string newPassword)
	{
		if (requestingUser.UserID != ID)
			if ((requestingUser.UserRole & UserRoleDTO.SystemAdministrator) != UserRoleDTO.SystemAdministrator)
				return Unauthorized();

		int rows = userService.SetPassword(ID, newPassword);
		if (rows == 1)
			return Ok("OK");
		else if (rows == 0)
			return NotFound();
		else
			throw new NTHException("SetPassword updated multiple rows");
	}

	[HttpPut]
	[Route("{ID}/UserRole")]
	public IActionResult SetUserRole(long ID, [FromBody] UserRoleDTO newUserRole)
	{
		UserRoleHistory? result = userService.SetUserRole(ID, newUserRole);
		if (result is null)
			return NotFound();
		return Ok(result);
	}

	// [HttpGet]
	// [Route("{ID}/Icon")]
	// [ResponseCache(Duration = 86400)]
	// public IActionResult GetUserIcon(long ID)
	// {
	// 	var iconID = database.Users.Where(x => x.ID == ID).Select(x => x.UserIconID).FirstOrDefault();
	// 	if (iconID == Guid.Empty)
	// 		return NotFound();
	// 	var info = database.UserIconHistories.AsNoTracking().FirstOrDefault(x => x.GUID == iconID);
	// 	if (info is null)
	// 		return NotFound();
	// 	byte[] image = info.Icon;
	// 	return File(image, "image/png", $"{info.UserID}-{info.CreationDate.ToString("s")}.png");
	// }

	[HttpPut]
	[Route("{ID}/Icon")]
	public async Task<IActionResult> SetUserIcon([FromRoute] long ID, IFormFile icon)
	{
		if (requestingUser.UserID != ID)
			if ((requestingUser.UserRole & UserRoleDTO.SystemAdministrator) != UserRoleDTO.SystemAdministrator)
				return Unauthorized();
		if (icon.Length < 5 || icon.Length > UserIconHistory.MAX_ICON_SIZE)
			return BadRequest("你想害我的库？");
		if (!database.Users.Any(x => x.ID == ID))
			return BadRequest("查无此人");
		if (string.IsNullOrEmpty(Path.GetExtension(icon.FileName)))
			return BadRequest("没后缀名啊");
		using Stream readStream = icon.OpenReadStream();
		byte[] bytes1 = new byte[icon.Length];
		await readStream.ReadExactlyAsync(bytes1);

		Image image;
		try { image = Image.Load(bytes1); }
		catch (Exception) { return BadRequest("什么破图？"); }
		using (image)
		{
			image.Size.Deconstruct(out int x, out int y);
			if (x != y)
				return BadRequest("不是正方形图片");
			if (x < 25)
				return BadRequest("太小");
			if (x > 800)
				return BadRequest("太大");

			DateTimeOffset newDate = DateTimeOffset.UtcNow;
			var historyItem = new UserIconHistory
			{
				UserID = ID,
				CreationDate = newDate,
			};
			var savedPath = Path.Join(UserCookieAssetController.UserIconPath, historyItem.GUID.ToString() + Path.GetExtension(icon.FileName).ToLower());
			await System.IO.File.WriteAllBytesAsync(savedPath, bytes1);
			// we don't solve high concurrency icon creation
			await database.UserIconHistories.AddAsync(historyItem);
			await database.SaveChangesAsync();
			database.Users.Where(x => x.ID == ID)
				.ExecuteUpdate(setter => setter
					.SetProperty(u => u.UserIconID, historyItem.GUID)
					.SetProperty(u => u.IconChangeDate, newDate));
			return Ok(historyItem.GUID);
		}
	}
}

/// <summary>
/// 希望所有的图片作为静态文件从GUID来获得
/// cookie验证
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("api/User")]
public class UserCookieAssetController : ControllerBase
{
	[HttpGet]
	[Route("Icon/{IconID}")]
	[ResponseCache(Duration = 60 * 60 * 24 * 30)]
	public IActionResult GetIconByIconID([FromRoute] Guid IconID)
	{
		if (IconID == default)
			return NotFound();
		List<string> extensions = ["png", "jpg", "jpeg", "webp"];
		foreach (var ext in extensions)
		{
			var potentialFile = Path.Join(UserIconPath, IconID.ToString() + '.' + ext);
			if (System.IO.File.Exists(potentialFile))
				return File(System.IO.File.Open(potentialFile, FileMode.Open), "image/" + ext);
		}
		return NotFound();
	}

	public static string UserIconPath = null!;
}

public partial class NonSensitiveUserDTO
{
	public long ID { get; set; }
	public required string Username { get; set; }
	#region Profile Icon
	public DateTimeOffset IconChangeDate { get; set; }
	#endregion Profile Icon

	#region Display name
	public required string Displayname { get; set; }
	public List<DisplaynameHistory>? DisplaynameHistory { get; set; }
	public DateTimeOffset DisplaynameChangeDate { get; set; }
	#endregion Display name

	#region TitleWords
	public string TitleWords { get; set; } = string.Empty;
	public DateTimeOffset TitleWordsChangeDate { get; set; }
	#endregion TitleWords

	#region User Roles
	public UserRoleDTO UserRole { get; set; }
	public List<UserRoleHistory>? UserRoleHistory { get; set; }
	public DateTimeOffset UserRoleChangeDate { get; set; }
	#endregion User Roles

	public DateTimeOffset CreationDate { get; set; }

	public static NonSensitiveUserDTO FromDBModel(UserID userID)
	{
		return new NonSensitiveUserDTO()
		{
			ID = userID.ID,
			Username = userID.Username,
			IconChangeDate = userID.IconChangeDate,
			Displayname = userID.Displayname,
			DisplaynameHistory = userID.DisplaynameHistory.Count > 0 ? userID.DisplaynameHistory : null,
			DisplaynameChangeDate = userID.DisplaynameChangeDate,
			TitleWords = userID.TitleWords,
			TitleWordsChangeDate = userID.TitleWordsChangeDate,
			UserRole = (UserRoleDTO)(int)userID.UserRole,
			UserRoleHistory = userID.UserRoleHistory.Count > 0 ? userID.UserRoleHistory : null,
			UserRoleChangeDate = userID.UserRoleChangeDate,
			CreationDate = userID.CreationDate
		};
	}
}
