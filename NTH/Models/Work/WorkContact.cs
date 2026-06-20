using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NTH.Models.Author;
using NTH.Models.User;

namespace NTH.Models.Work;

/// <summary>
/// This should be called WorkContactHistory because it uses history storage style as well.
/// Query from the Author and the one with new biggest ID is the current data.
/// </summary>
[Index(nameof(AuthorID), nameof(ID))]
public class WorkContact
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }

	public DateTimeOffset ChangeDate { get; set; } = DateTimeOffset.UtcNow;

	#region User
	[Column(name: "UserID")]
	public long UserID { get; set; }
	[Column(name: "UserID")]
	[JsonIgnore]
	public UserID? User { get; set; }
	#endregion User

	#region Author
	[Column(name: "AuthorID")]
	public long AuthorID { get; set; }
	[Column(name: "AuthorID")]
	public AuthorID? Author { get; set; }
	#endregion Author
}
