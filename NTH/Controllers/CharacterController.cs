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
			Age = newChara.Age,
			Mingzi = newChara.Mingzi,
			Bieming = newChara.Bieming
		};
		database.CharacterReality.Add(newCharacter);
		try { database.SaveChanges(); }
		catch (DbUpdateException ex) when (ex.InnerException is not null && ex.InnerException.Message.StartsWith("23505"))
		{ return BadRequest("Character already exists."); }
		return CreatedAtAction(nameof(CreateCharacter), newCharacter);
	}

	[HttpPut]
	public IActionResult ModifyCharacter(NewCharacter character)
	{
		CharacterID? existing = Reality.CharactersByName.FirstOrDefault(x => x.Key == character.Nameue + character.Nameshita).Value;
		if (existing is not null)
			return BadRequest("Fixed character");
		existing = database.CharacterReality.FirstOrDefault(x => x.Nameue == character.Nameue && x.Nameshita == character.Nameshita);
		if (existing is null)
			return NotFound();
		if (character.Introduction.Length > 0)
			existing.Introduction = character.Introduction;
		if (character.Age.HasValue)
			existing.Age = character.Age;
		if (character.Mingzi.Length > 0)
			existing.Mingzi = character.Mingzi;
		if (character.Bieming.Count > 0 || character.DeleteBieming.Count > 0)
		{
			SortedSet<string> xinbieming = [.. existing.Bieming, .. character.Bieming];
			foreach (var item in character.DeleteBieming)
			{
				xinbieming.Remove(item);
			}
			existing.Bieming = [.. xinbieming];
		}
		database.SaveChanges();
		return Ok("OK");
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
	[MaxLength(25)]
	public string Mingzi { get; set; } = string.Empty;
	public List<string> Bieming { get; set; } = [];
	public List<string> DeleteBieming { get; set; } = [];
}
