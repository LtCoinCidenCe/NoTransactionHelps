using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NTH.Models.CharacterReality;

[PrimaryKey(nameof(Nameue), nameof(Nameshita))]
public class CharacterID
{
	#region Database
	[MaxLength(25)]
	public required string Nameue { get; set; }
	[MaxLength(25)]
	public string Nameshita { get; set; } = string.Empty;
	[MaxLength(25)]
	public string IconFilename { get; set; } = string.Empty; // let it be a static file
	[MaxLength(500)]
	public string Introduction { get; set; } = string.Empty;
	public int? Age { get; set; }
	[MaxLength(25)]
	public string Mingzi { get; set; } = string.Empty;
	public List<string> Bieming { get; set; } = [];
	#endregion Database

	#region Calculated Properties
	/// <summary>
	/// help the memory to mark it as a code stored character
	/// </summary>
	public bool fixedChara = false;
	public bool IsFixed { get => fixedChara; }
	public string FullName
	{
		get => Nameue + Nameshita;
	}
	#endregion Calculated Properties
}
