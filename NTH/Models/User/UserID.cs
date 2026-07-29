using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NTH.Models.Work;

namespace NTH.Models.User;

[Index(nameof(Username), IsUnique = true)]
public partial class UserID
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	// [Length(2, 30)] this doesn't work for DB
	[MaxLength(30)]
	public required string Username { get; set; }

	#region Profile Icon
	/// <summary>
	/// Since user icon can be too big (3MB)
	/// An indirect query is required to reduce the load.
	/// Don't even .Include this History as this could be painful
	/// </summary>
	public List<UserIconHistory> UserIconHistory { get; set; } = new();
	/// <summary>
	/// Since going to UserIconHistories can be painful.
	/// Just store a value here to quickly find the UserIcon
	/// </summary>
	public Guid UserIconID { get; set; }
	public DateTimeOffset IconChangeDate { get; set; } = DateTimeOffset.UtcNow;
	#endregion Profile Icon

	#region Display name
	// [Length(2, 30)] this doesn't work for DB
	[MaxLength(30)]
	public required string Displayname { get; set; }
	public List<DisplaynameHistory> DisplaynameHistory { get; set; } = new();
	public DateTimeOffset DisplaynameChangeDate { get; set; } = DateTimeOffset.UtcNow;
	#endregion Display name

	#region TitleWords
	[MaxLength(250)]
	public string TitleWords { get; set; } = string.Empty;
	public DateTimeOffset TitleWordsChangeDate { get; set; } = DateTimeOffset.UtcNow;
	#endregion TitleWords

	#region Password
	[MaxLength(32)]
	public required byte[] Password { get; set; }
	[MaxLength(5)]
	public string PassSalt { get; set; } = "     ";
	public DateTimeOffset PasswordChangeDate { get; set; } = DateTimeOffset.UtcNow;
	#endregion Password

	#region User Roles
	public UserRoleDTO UserRole { get; set; }
	public List<UserRoleHistory> UserRoleHistory { get; set; } = new();
	public DateTimeOffset UserRoleChangeDate { get; set; } = DateTimeOffset.UtcNow;
	#endregion User Roles

	#region AllWorks
	public List<WorkContact> Contact { get; set; } = new();
	public List<WorkID> Works { get; set; } = new();
	#endregion AllWorks

	public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
	public bool IsDeleted { get; set; } = false;
}
