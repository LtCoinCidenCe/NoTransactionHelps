using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.Author;
using NTH.Models.Author;
using NTH.Services;

namespace NTH.Controllers;

[ApiController]
[Route("api/Author")]
public class AuthorController(ILogger<AuthorController> logger, PostgresContext database, AuthorService authorService) : ControllerBase
{
    [HttpGet, Authorize]
    public IActionResult GetAllAuthors()
    {
        var query = database.Authors.Include(x => x.Contact)
            .Select(author => new
            {
                author.ID,
                author.Name,
                author.YoutubeHomePage,
                author.NiconicoHomePage,
                author.BilibiliHomePage,
                author.TwitterHomePage,
                author.AuthorizedPerVideo,
                author.AllVideoAuthorized,
                author.AuthorizationChangeDate,
                author.AdditionalRequirements,
                author.AdditionalRequirementsChangeDate,
                author.CreationDate,
                ContactUserID = author.Contact.Select(x => x.UserID).FirstOrDefault()
            });
        return Ok(query);
    }

    /// <summary>
    /// Register a new author. Name should be unique but too lazy to check with mutex.
    /// </summary>
    /// <returns></returns>
    [HttpPost, Authorize]
    public ActionResult<AuthorID> CreateNewAuthor(NewAuthorDTO newAuthorDTO)
    {
        AuthorID author = AuthorID.FromDTO(newAuthorDTO);
        bool existing = database.Authors.AsNoTracking().Any(x => x.Name == author.Name);
        if (existing)
            return BadRequest();
        database.Authors.Add(author);
        database.SaveChanges();
        return CreatedAtAction(nameof(CreateNewAuthor), author);
    }
}
