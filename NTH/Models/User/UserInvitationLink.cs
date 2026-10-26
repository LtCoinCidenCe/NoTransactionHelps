using System.ComponentModel.DataAnnotations;

namespace NTH.Models.User;

public class UserInvitationLink
{
	public long ID { get; set; }

	#region Invitation
	public long ByUserAudit { get; set; }
	public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
	public int ConfidentNumber { get; set; }
	#endregion Invitation

	public required byte[] Key { get; set; }
	/// <summary>
	/// Encryption Initialization Vector
	/// </summary>
	[MaxLength(32)]
	public required byte[] IV { get; set; }

	public long? CreatedUser { get; set; } = null;
}
