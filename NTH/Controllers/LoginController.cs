using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Login(UserLoginDTO userLoginDTO)
    {
        var user = userService.Login(userLoginDTO);
        if (user is null)
            return BadRequest();
        string? salt = user.PassSalt;
        byte[] calculatedPasshash = PasswordHasher.GetHashedPassword(userLoginDTO.Password, ref salt);
        if (!calculatedPasshash.SequenceEqual(user.Password))
            return BadRequest();
        return Ok(); // TODO
    }
}
