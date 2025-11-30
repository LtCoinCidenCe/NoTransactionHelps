using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Services;
using NTH.Utilities;

namespace NTH.Controllers;

[ApiController]
[Route("api/Login")]
public class LoginController(ILogger<LoginController> logger, PostgresContext database, UserService userService) : ControllerBase
{
    [HttpPost]
    public ActionResult<string> Login(UserLoginDTO userLoginDTO)
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
        return Ok(token);
    }
}
