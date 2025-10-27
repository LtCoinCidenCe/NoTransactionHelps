using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.User;
using NTH.Services;

[ApiController]
[Route("api/User")]
public class UserController(ILogger<UserController> logger, PostgresContext database, UserService userService) : ControllerBase
{
    [HttpGet]
    [Route("{ID}")]
    public ActionResult<NonSensitiveUserDTO> GetUser(string ID)
    {
        var user = userService.GetUserByID(ID);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user.toDTO());
    }

    [HttpPost]
    public IActionResult CreateNewUser(NewUserDTO newUser)
    {
        UserID? newUserID = userService.CreateNewUser(newUser);
        if (newUserID is null)
            return BadRequest();
        return CreatedAtAction(nameof(CreateNewUser), newUserID);
    }

    [HttpPut]
    [Route("{ID}/DisplayName")]
    public IActionResult SetDisplayName(long ID, [Length(2, 30)][FromBody] string newDisplayName)
    {
        DisplaynameHistory? result = userService.SetDisplayName(ID, newDisplayName, null);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPut]
    [Route("{ID}/TitleWords")]
    public IActionResult SetTitleWords(long ID, [MaxLength(250)][FromBody] string newTitleWords)
    {
        int rows = userService.SetTitleWords(ID, newTitleWords);
        if (rows == 1)
            return Ok();
        else if (rows == 0)
            return NotFound();
        else
            throw new Exception("SetTitleWords updated multiple rows");
    }

    [HttpPut]
    [Route("{ID}/Password")]
    public IActionResult SetPassword(long ID, [MinLength(5)][FromBody] string newPassword)
    {
        int rows = userService.SetPassword(ID, newPassword);
        if (rows == 1)
            return Ok();
        else if (rows == 0)
            return NotFound();
        else
            throw new Exception("SetPassword updated multiple rows");
    }

    [HttpPut]
    [Route("{ID}/UserRole")]
    public IActionResult SetUserRole(long ID, [FromBody] UserRole newUserRole)
    {
        UserRoleHistory? result = userService.SetUserRole(ID, newUserRole);
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}
