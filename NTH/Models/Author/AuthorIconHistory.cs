using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NTH.Models.Author;

[PrimaryKey(nameof(GUID))]
[Index(nameof(CreationDate))]
[Index(nameof(AuthorID), nameof(ID), IsUnique = true)]
public class AuthorIconHistory
{
	public const int MAX_ICON_SIZE = 3_000_000; // 3MB

	public Guid GUID { get; set; } = Guid.CreateVersion7();

	[Column(name: "AuthorID")]
	public long AuthorID { get; set; }
	[Column(name: "AuthorID"), JsonIgnore]
	public AuthorID? Author { get; set; }

	// #region itsumono
	// let's go little bit different for this "file" like table
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long ID { get; set; }

	public long ByUserAudit { get; set; }

	public required DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

	[JsonIgnore]
	public bool IsDeleted { get; set; } = false;
	// #endregion itsumono

	[MaxLength(MAX_ICON_SIZE)]
	public byte[] Icon { get; set; } = [];
}
