using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NTH.Controllers;

[ApiController]
[Route("api/Login")]
public class LoginController(ILogger<LoginController> logger, SQLiteContext database, UserService userService) : ControllerBase
{
	[HttpPost]
	public async Task<ActionResult<string>> Login(UserLoginDTO userLoginDTO)
	{
		var user = userService.Login(userLoginDTO);
		if (user is null)
			return BadRequest();
		bool isSA = (user.UserRole & UserRoleDTO.SystemAdministrator) != 0;
		string roleString = isSA ? "sa" : "kt";
		string? salt = user.PassSalt;
		byte[] calculatedPasshash = PasswordHasher.GetHashedPassword(userLoginDTO.Password, ref salt);
		if (!calculatedPasshash.SequenceEqual(user.Password))
			return BadRequest();
		var jwt = new JwtSecurityToken(
			JwtHelper.ISSUER,
			$"{roleString}{user.UserID}",
			null,
			notBefore: null,
			expires: DateTime.Now + TimeSpan.FromMinutes(15),
			new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtHelper.SECRET)),
			SecurityAlgorithms.HmacSha256));
		var token = new JwtSecurityTokenHandler().WriteToken(jwt);

		// for cookie
		var claims = new List<Claim>
		{
			new Claim("aud", user.UserID.ToString()),
			new Claim(ClaimTypes.Role, roleString)
		};
		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
		return Ok(token);
	}

	[HttpPost, Route("InvitationLink"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> GenerateInvitationLink([FromServices] RequestingUser requestingUser)
	{
		var dateTimeOffset = DateTimeOffset.UtcNow;
		var clearText = new InvitationToken
		{
			ByUserAudit = requestingUser.UserID,
			CreationDate = dateTimeOffset,
		};
		var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(clearText);
		var aes0 = Aes.Create();

		var initializationVector = aes0.IV;
		byte[] buffer = new byte[300];
		var successful = aes0.TryEncryptCfb(jsonBytes, initializationVector, buffer, out var written);
		if (!successful)
		{
			logger.LogError("encryption failed when preparing invitation token");
			return Problem();
		}
		Array.Resize(ref buffer, written);
		var salasana = Convert.ToBase64String(buffer, 0, written);

		var dbInvite = new UserInvitationLink
		{
			ByUserAudit = requestingUser.UserID,
			CreationDate = dateTimeOffset,
			IV = initializationVector,
		};

		var linkBuilder = new UriBuilder
		{
			Scheme = Request.Scheme,
			Host = Request.Host.ToUriComponent(),
			Path = "Invited",
			Query = $"?ID={0}&token={salasana}"
		};
		var ljnk = linkBuilder.ToString();

		return Ok(ljnk);
	}

	[HttpGet, Route("InvitationTokenValidation")]
	public async Task<IActionResult> InvitationTokenValidation
		([FromQuery, Required] long ID, [FromQuery, Required, MinLength(14)] string token)
	{
		var salasana = Convert.FromBase64String(token);
		var aes0 = Aes.Create();
		return NotFound();
	}

	[HttpGet, Route("InvitedAccountCreation")]
	public async Task<IActionResult> InvitedAccountCreation
		([FromQuery, Required] long ID, [FromQuery, Required, MinLength(14)] byte[] token)
	{
		return NotFound();
	}

	[HttpDelete, Route("salainen/shutdown"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public IActionResult ShutdownProgram([FromServices] RequestingUser requestingUser)
	{
		// 因为SQLite需要优雅关机
		if (requestingUser.UserID != 1) // 超级用户权力大，好的有用都给他
			return NotFound();
		Program.app.StopAsync();
		return Ok("OK");
	}
}

public class InvitationToken
{
	public long ByUserAudit { get; set; }
	public DateTimeOffset CreationDate { get; set; }
}
