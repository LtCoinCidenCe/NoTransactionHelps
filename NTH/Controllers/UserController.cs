using System.Collections;
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

namespace NTH.Controllers;

[ApiController]
[Route("api/User")]
public class UserController(ILogger<UserController> logger, PostgresContext database, UserService userService) : ControllerBase
{
    [HttpGet, Authorize]
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

    [HttpGet, Authorize]
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
            return Ok("OK");
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
            return Ok("OK");
        else if (rows == 0)
            return NotFound();
        else
            throw new Exception("SetPassword updated multiple rows");
    }

    [HttpPut, Authorize]
    [Route("{ID}/UserRole")]
    public IActionResult SetUserRole(long ID, [FromBody] UserRoleDTO newUserRole)
    {
        UserRoleHistory? result = userService.SetUserRole(ID, newUserRole);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [Route("{ID}/Icon")]
    [ResponseCache(Duration = 86400)]
    public IActionResult GetUserIcon(long ID)
    {
        long iconID = database.Users.Where(x => x.ID == ID).Select(x => x.UserIconID).FirstOrDefault();
        if (iconID == 0)
            return NotFound();
        var info = database.UserIconHistories.AsNoTracking().FirstOrDefault(x => x.ID == iconID);
        if (info is null)
            return NotFound();
        byte[] image = info.Icon;
        return File(image, "image/png", $"{info.UserID}-{info.CreationDate.ToString("s")}.png");
    }

    [HttpPut, Authorize]
    [Route("{ID}/Icon")]
    public IActionResult SetUserIcon([FromRoute] long ID, IFormFile file)
    {
        if (!ControllerHelper.CheckUserClaimsID(User, ID))
            return Unauthorized();
        if (file.Length < 5 || file.Length > UserIconHistory.MAX_ICON_SIZE)
            return BadRequest();
        if (!database.Users.Any(x => x.ID == ID))
            return BadRequest();
        Stream readStream = file.OpenReadStream();

        Image image;
        try { image = Image.Load(readStream); }
        catch (Exception) { return BadRequest("Image file reading error"); }
        using (image)
        {
            image.Size.Deconstruct(out int x, out int y);
            if (x != y)
                return BadRequest("Not square Image");
            if (x < 25)
                return BadRequest("Image too small");
            if (x > 800)
                return BadRequest("Image too big");
            using MemoryStream pngStream = new();
            image.SaveAsPng(pngStream);
            byte[] bytes = pngStream.ToArray();
            if (bytes.Length > UserIconHistory.MAX_ICON_SIZE)
                return BadRequest();
            DateTimeOffset newDate = DateTimeOffset.UtcNow;
            var historyItem = new UserIconHistory
            {
                UserID = ID,
                Icon = bytes,
                CreationDate = newDate,
            };
            // we don't solve high concurrency icon creation
            database.UserIconHistories.Add(historyItem);
            database.SaveChanges();
            database.Users.Where(x => x.ID == ID)
                .ExecuteUpdate(setter => setter
                    .SetProperty(u => u.UserIconID, historyItem.ID)
                    .SetProperty(u => u.IconChangeDate, newDate));
            return Ok("OK");
        }
    }
}
