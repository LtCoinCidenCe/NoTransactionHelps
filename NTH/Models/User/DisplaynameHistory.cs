using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.User;

[Index(nameof(CreationDate))]
[Index(nameof(UserID), nameof(ID), IsUnique = true)]
public partial class DisplaynameHistory
{
	#region itsumono
	public long ID { get; set; }

	public long ByUserAudit { get; set; }

	[Column(name: "UserID")]
	public long UserID { get; set; }
	[Column(name: "UserID")]
	[JsonIgnore]
	public UserID? User { get; set; }

	public required DateTimeOffset CreationDate { get; set; }

	[JsonIgnore]
	public bool IsDeleted { get; set; } = false;
	#endregion itsumono

	[MaxLength(30)]
	public required string Displayname { get; set; }
}
