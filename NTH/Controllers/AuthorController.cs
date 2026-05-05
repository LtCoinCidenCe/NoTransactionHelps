using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Models.Author;
using NTH.Models.Work;
using NTH.Services;

namespace NTH.Controllers;

[ApiController]
[Route("api/Author")]
public class AuthorController(ILogger<AuthorController> logger, PostgresContext database, AuthorService authorService) : ControllerBase
{
	[HttpGet, Authorize]
	public IActionResult GetAllAuthors()
	{
		var data = database.Authors.Include(x => x.Contact)
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
				ContactUserIDraw = author.Contact.Select(x => x.UserID).ToList()
			}).ToList();

		return Ok(data.Select(x => new { x.ID, x.Name, x.YoutubeHomePage, x.NiconicoHomePage, x.BilibiliHomePage, x.TwitterHomePage, x.AuthorizedPerVideo, x.AllVideoAuthorized, x.AuthorizationChangeDate, x.AdditionalRequirements, x.AdditionalRequirementsChangeDate, x.CreationDate, ContactUserID = x.ContactUserIDraw.FirstOrDefault() }));
	}

	/// <summary>
	/// Register a new author. Name should be unique but too lazy to check with mutex.
	/// </summary>
	/// <returns></returns>
	[HttpPost, Authorize]
	public ActionResult<AuthorID> CreateNewAuthor(NewAuthorDTO newAuthorDTO, [FromServices] RequestingUser requestingUser)
	{
		bool existing = database.Authors.AsNoTracking().Any(x => x.Name == newAuthorDTO.Name);
		if (existing)
			return BadRequest();
		AuthorID author = newAuthorDTO.ToDBModel();
		author.ByUserAudit = requestingUser.UserID;
		database.Authors.Add(author);
		database.SaveChanges();
		return CreatedAtAction(nameof(CreateNewAuthor), author);
	}

	[HttpPut, Authorize]
	[Route("{authorID}/Requirements")]
	public ActionResult<AdditionalRequirementsHistory> SetAuthorRequirements(long authorID, [FromBody, MaxLength(800)] string requirements)
	{
		// avoid fetching the author row with its big Icon
		bool existing = database.Authors.Any(x => x.ID == authorID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;
		var newHistory = new AdditionalRequirementsHistory()
		{
			AuthorID = authorID,
			TensaiRequirements = requirements,
			CreationDate = timeNow
		};
		database.AdditionalRequirementsHistories.Add(newHistory);
		database.SaveChanges();
		database.Authors
			.Where(x => x.ID == authorID)
			.ExecuteUpdate(setter => setter
				.SetProperty(t => t.AdditionalRequirements, requirements)
				.SetProperty(t => t.AdditionalRequirementsChangeDate, timeNow));
		return Ok(newHistory);
	}

	[HttpPut, Authorize]
	[Route("{authorID}/Authorization")]
	public ActionResult<AuthorizationChangeHistory> SetAuthorization(long authorID, [FromBody] AuthorizationChangeDTO newAuth)
	{
		// avoid fetching the author row with its big Icon
		bool existing = database.Authors.Any(x => x.ID == authorID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;
		var newHistory = new AuthorizationChangeHistory()
		{
			AuthorizedPerVideo = newAuth.AuthorizedPerVideo,
			AllVideoAuthorized = newAuth.AllVideoAuthorized,
			AuthorID = authorID,
			CreationDate = timeNow
		};
		database.AuthorizationChangeHistories.Add(newHistory);
		database.SaveChanges();
		database.Authors
			.Where(x => x.ID == authorID)
			.ExecuteUpdate(setter => setter
				.SetProperty(t => t.AllVideoAuthorized, newAuth.AllVideoAuthorized)
				.SetProperty(t => t.AuthorizedPerVideo, newAuth.AuthorizedPerVideo)
				.SetProperty(t => t.AuthorizationChangeDate, timeNow));
		return Ok(newHistory);
	}

	[HttpPut, Authorize]
	[Route("{authorID}/Contact")]
	public ActionResult<WorkContact> SetContact(long authorID, [FromBody] long userID)
	{
		var atr = database.Authors
			.AsNoTracking()
			.Include(x => x.Contact)
			.Where(x => x.ID == authorID)
			.Select(x => new { x.ID, x.Name, x.Contact, x.CreationDate })
			.FirstOrDefault();
		if (atr is null)
			return NotFound();
		bool existing = database.Users.Any(x => x.ID == userID);
		if (!existing)
			return NotFound();
		var timeNow = DateTimeOffset.UtcNow;
		var contactRelation = atr.Contact.FirstOrDefault();
		if (contactRelation is null)
		{
			var newContact = new WorkContact()
			{
				ChangeDate = timeNow,
				UserID = userID,
				AuthorID = authorID
			};
			database.WorkContacts.Add(newContact);
			database.SaveChanges();
			return Ok(newContact);
		}
		else
		{
			database.WorkContacts
				.Where(x => x.ID == contactRelation.ID)
				.ExecuteUpdate(setter => setter
					.SetProperty(c => c.UserID, userID)
					.SetProperty(c => c.ChangeDate, timeNow));
			contactRelation.UserID = userID;
			contactRelation.ChangeDate = timeNow;
			return Ok(contactRelation);
		}
	}
}

public class AuthorizationChangeDTO
{
	public required bool AuthorizedPerVideo { get; set; }
	public required bool AllVideoAuthorized { get; set; }
}

/// <summary>
/// For minimal requirement, write only Name
/// </summary>
public class NewAuthorDTO
{
	[MaxLength(30)]
	public required string Name { get; set; }
	[MaxLength(200)]
	public string YoutubeHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string NiconicoHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string BilibiliHomePage { get; set; } = string.Empty;
	[MaxLength(200)]
	public string TwitterHomePage { get; set; } = string.Empty;
	public bool AuthorizedPerVideo { get; set; } = false;
	public bool AllVideoAuthorized { get; set; } = false;
	[MaxLength(800)]
	public string TensaiRequirement { get; set; } = string.Empty;
}

public static class NewAuthorDTOExtension
{
	/// <summary>
	/// be aware this returned object contains AdditionalRequirementsHistory and AuthorizationChangeHistory
	/// </summary>
	/// <returns></returns>
	public static AuthorID ToDBModel(this NewAuthorDTO newAuthorDTO)
	{
		var datetime = DateTimeOffset.UtcNow;
		var newAuthor = new AuthorID()
		{
			Name = newAuthorDTO.Name,
			YoutubeHomePage = newAuthorDTO.YoutubeHomePage,
			NiconicoHomePage = newAuthorDTO.NiconicoHomePage,
			BilibiliHomePage = newAuthorDTO.BilibiliHomePage,
			TwitterHomePage = newAuthorDTO.TwitterHomePage,
			AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
			AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
			AuthorizationChangeDate = datetime,
			AdditionalRequirements = newAuthorDTO.TensaiRequirement,
			AdditionalRequirementsChangeDate = datetime,
			CreationDate = datetime
		};
		newAuthor.AdditionalRequirementsHistory.Add(new()
		{
			CreationDate = datetime,
			TensaiRequirements = newAuthorDTO.TensaiRequirement
		});
		newAuthor.AuthorizationChangeHistory.Add(new()
		{
			AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
			AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
			CreationDate = datetime
		});
		return newAuthor;
	}
}
