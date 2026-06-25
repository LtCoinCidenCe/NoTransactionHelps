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
	#endregion Database

	#region Calculated Properties
	public string FullName
	{
		get => Nameue + Nameshita;
	}
	#endregion Calculated Properties
}
