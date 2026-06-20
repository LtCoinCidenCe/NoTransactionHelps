using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NTH.Models.Author;

public class AdditionalRequirementsHistory
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	[Column(name: "AuthorID")]
	public long AuthorID { get; set; }
	[Column(name: "AuthorID")]
	[JsonIgnore]
	public AuthorID? Author { get; set; }
	public required DateTimeOffset CreationDate { get; set; }
	[JsonIgnore]
	public bool IsDeleted { get; set; } = false;

	[MaxLength(800)]
	public string TensaiRequirements { get; set; } = string.Empty;
}
