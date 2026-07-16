using System.ComponentModel.DataAnnotations;

namespace NTH.Models.User;

public class UserInvitationLink
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
	public bool IsUsed { get; set; } = false;
	/// <summary>
	/// Encryption Initialization Vector
	/// </summary>
	[MaxLength(32)]
	public required byte[] IV { get; set; }
	[MaxLength(400)]
	public string URLGenerated { get; set; } = "";
	public long? CreatedUser { get; set; } = null;
}
