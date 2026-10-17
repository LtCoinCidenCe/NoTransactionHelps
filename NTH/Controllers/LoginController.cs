using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
		using var aes0 = Aes.Create();

		byte[] buffer = new byte[300];
		var successful = aes0.TryEncryptCfb(jsonBytes, aes0.IV, buffer, out var written);
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
			Key = aes0.Key,
			IV = aes0.IV,
			ConfidentNumber = clearText.ConfidentNumber
		};

		database.UserInvitationLinks.Add(dbInvite);
		database.SaveChanges();

		var linkBuilder = new UriBuilder
		{
			Scheme = Request.Scheme,
			Host = Request.Host.ToUriComponent(),
			Path = "Invited",
			Query = $"?ID={dbInvite.ID}&token={salasana}"
		};
		var ljnk = linkBuilder.ToString();
		// http://localhost/Invited?ID=1&token=GxnP84ETqcSos+iBDY3RBpQ6YVIM2H4vC8G87pzkgPLL988W6DjMG+s7osotCl8ILTxvQjygLaXGnUYuUd5y3RAAkwLB6y9ZDHkTgXk6wX0Q3D/+zE1fXJFPw3n6ij6I

		return Ok(ljnk);
	}

	[HttpGet, Route("InvitationTokenValidation")]
	public async Task<IActionResult> InvitationTokenValidation
		([FromQuery, Required] long ID, [FromQuery, Required, MinLength(14), MaxLength(300)] string token)
	{
		var dbToken = await database.UserInvitationLinks.AsNoTracking().FirstOrDefaultAsync(x => x.ID == ID);
		if (dbToken is null)
			return NotFound();
		if (dbToken.CreatedUser is not null)
			return BadRequest("Used invitation");
		byte[] salasana;
		try { salasana = Convert.FromBase64String(token); }
		catch (FormatException) { return BadRequest(); }

		using var aes0 = Aes.Create();
		aes0.Key = dbToken.Key;
		byte[] decryption = new byte[300];

		var ok = aes0.TryDecryptCfb(salasana, dbToken.IV, decryption, out var bytesLength);
		if (!ok)
			return BadRequest("What are you trying to do?");
		Array.Resize(ref decryption, bytesLength);

		InvitationToken? inviT;
		try { inviT = JsonSerializer.Deserialize<InvitationToken>(decryption) ?? throw new JsonException("null is not what I wanted"); }
		catch (JsonException) { return BadRequest("It must be correct json"); }

		if (dbToken.ByUserAudit != inviT.ByUserAudit || dbToken.CreationDate != inviT.CreationDate || dbToken.ConfidentNumber != inviT.ConfidentNumber)
			return BadRequest("Where the heck did you get the token from?");
		if (DateTimeOffset.UtcNow - inviT.CreationDate > TimeSpan.FromDays(10))
			return BadRequest("invitation too old");
		return Ok("OK");
	}

	[HttpPost, Route("InvitedAccountCreation")]
	public async Task<IActionResult> InvitedAccountCreation(AccountCreationDTO accountCreationDTO)
	{
		var result = await InvitationTokenValidation(accountCreationDTO.ID, accountCreationDTO.Token);
		if (result is not OkObjectResult)
			return result;
		var userID = userService.CreateNewUser(accountCreationDTO.NewUser) ?? throw new NTHException("I don't know why user is not created");
		var i = database.UserInvitationLinks.Where(x => x.ID == accountCreationDTO.ID)
			.ExecuteUpdate(setter => setter.SetProperty(y => y.CreatedUser, userID.ID));
		return Ok("OK");
	}

	[HttpDelete, Route("salainen/shutdown"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public IActionResult ShutdownProgram([FromServices] RequestingUser requestingUser, [FromServices] YtdlpInstanceService ytdlpInstanceService)
	{
		// 因为SQLite需要优雅关机
		if (requestingUser.UserID != 1) // 超级用户权力大，好的有用都给他
			return NotFound();
		ytdlpInstanceService.ChannelClosing();
		YtdlpInstanceService.TaskStation.Wait();
		Program.app.StopAsync();
		return Ok("OK");
	}
}

public class InvitationToken
{
	public long ByUserAudit { get; set; }
	public DateTimeOffset CreationDate { get; set; }
	public int ConfidentNumber { get; set; } = Random.Shared.Next();
}

public class AccountCreationDTO
{
	[Required]
	public long ID { get; set; }
	[Required, MinLength(14), MaxLength(300)]
	public required string Token { get; set; }
	public required NewUserDTO NewUser { get; set; }
}
