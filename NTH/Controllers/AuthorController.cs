using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.DTO.Author;
using NTH.Models.Author;

[ApiController]
[Route("api/Author")]
public class AuthorController(ILogger<AuthorController> logger, PostgresContext database) : ControllerBase
{
    [HttpGet]
    public List<AuthorID> GetAllAuthors()
    {
        return database.Authors.AsNoTracking().ToList();
    }

    /// <summary>
    /// Register a new author
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public ActionResult<AuthorID> CreateNewAuthor(NewAuthorDTO newAuthorDTO)
    {
        AuthorID author = AuthorID.FromDTO(newAuthorDTO);
        database.Authors.Add(author);
        database.SaveChanges();
        return CreatedAtAction(nameof(CreateNewAuthor), author);
    }
}
