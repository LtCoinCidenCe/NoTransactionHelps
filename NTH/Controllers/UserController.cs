using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.User;
using NTH.Models.User;
using NTH.Services;
using NTH.Utilities;
using SixLabors.ImageSharp;

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
        return Ok(user.ToDTO());
    }

    [HttpPost]
    public ActionResult<NonSensitiveUserDTO> CreateNewUser(NewUserDTO newUser)
    {
        UserID? newUserID = userService.CreateNewUser(newUser);
        if (newUserID is null)
            return BadRequest();
        return CreatedAtAction(nameof(CreateNewUser), newUserID.ToDTO());
    }

    [HttpPut, Authorize]
    [Route("{ID}/DisplayName")]
    public IActionResult SetDisplayName(long ID, [Length(2, 30)][FromBody] string newDisplayName)
    {
        if (!ControllerHelper.CheckUserClaimsID(User, ID))
            return Unauthorized();

        DisplaynameHistory? result = userService.SetDisplayName(ID, newDisplayName, null);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPut, Authorize]
    [Route("{ID}/TitleWords")]
    public IActionResult SetTitleWords(long ID, [MaxLength(250)][FromBody] string newTitleWords)
    {
        if (!ControllerHelper.CheckUserClaimsID(User, ID))
            return Unauthorized();

        int rows = userService.SetTitleWords(ID, newTitleWords);
        if (rows == 1)
            return Ok();
        else if (rows == 0)
            return NotFound();
        else
            throw new Exception("SetTitleWords updated multiple rows");
    }

    [HttpPut, Authorize]
    [Route("{ID}/Password")]
    public IActionResult SetPassword(long ID, [MinLength(5)][FromBody] string newPassword)
    {
        if (!ControllerHelper.CheckUserClaimsID(User, ID))
            return Unauthorized();

        int rows = userService.SetPassword(ID, newPassword);
        if (rows == 1)
            return Ok();
        else if (rows == 0)
            return NotFound();
        else
            throw new Exception("SetPassword updated multiple rows");
    }

    [HttpPut, Authorize]
    [Route("{ID}/UserRole")]
    public IActionResult SetUserRole(long ID, [FromBody] UserRoleDTO newUserRole)
    {
        // TODO here I set it to self assign roles
        if (!ControllerHelper.CheckUserClaimsID(User, ID))
            return Unauthorized();

        UserRoleHistory? result = userService.SetUserRole(ID, newUserRole);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [Route("{ID}/Icon")]
    public IActionResult GetUserIcon(long ID)
    {
        byte[]? image = database.Users.AsNoTracking().Where(x => x.ID == ID).Select(x => x.Icon).FirstOrDefault();
        if (image is null)
            return NotFound();
        return File(image, "image/png");
    }

    [HttpPost]
    [Route("{ID}/Icon")]
    public IActionResult SetUserIcon([FromRoute] long ID, IFormFile file)
    {
        if (file.Length < 5 || file.Length > UserID.MAX_ICON_SIZE)
            return BadRequest();
        if (!database.Users.Any(x => x.ID == ID))
            return BadRequest();
        Stream readStream = file.OpenReadStream();
        Image image = Image.Load(readStream);
        MemoryStream pngStream = new MemoryStream();
        image.SaveAsPng(pngStream);
        byte[] bytes = pngStream.ToArray();
        database.Users.Where(x => x.ID == ID).ExecuteUpdate(setter => setter.SetProperty(x => x.Icon, bytes));
        return Ok();
    }
}
