using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.CharacterReality;

namespace NTH.Controllers;

[Authorize, ApiController, Route("api/Character")]
public class CharacterController(PostgresContext database) : ControllerBase
{
	[HttpGet]
	public ImmutableList<CharacterID> GetCharacters()
	{
		ImmutableList<CharacterID> allCharas = Reality.Characters.AddRange(database.CharacterReality);

		return allCharas;
	}

	[HttpPost]
	public IActionResult CreateCharacter(NewCharacter newChara)
	{
		var inProgram = Reality.CharactersByName.ContainsKey(newChara.Nameue + newChara.Nameshita);
		if (inProgram)
			return BadRequest("Character already exists.");
		var newCharacter = new CharacterID()
		{
			Nameue = newChara.Nameue,
			Nameshita = newChara.Nameshita,
			Introduction = newChara.Introduction,
			Age = newChara.Age
		};
		database.CharacterReality.Add(newCharacter);
		try { database.SaveChanges(); }
		catch (DbUpdateException ex) when (ex.InnerException is not null && ex.InnerException.Message.StartsWith("23505"))
		{ return BadRequest("Character already exists."); }
		return CreatedAtAction(nameof(CreateCharacter), newCharacter);
	}
}

public class NewCharacter
{
	[MaxLength(25)]
	public required string Nameue { get; set; }
	[MaxLength(25)]
	public string Nameshita { get; set; } = string.Empty;
	[MaxLength(500)]
	public string Introduction { get; set; } = string.Empty;
	public int? Age { get; set; }
}
