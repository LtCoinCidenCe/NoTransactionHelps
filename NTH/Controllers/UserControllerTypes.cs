using NTH.Models.User;

namespace NTH.Controllers;

public class NonSensitiveUserDTO
{
	public long ID { get; set; }
	public required string Username { get; set; }
	#region Profile Icon
	public DateTimeOffset IconChangeDate { get; set; }
	#endregion Profile Icon

	#region Display name
	public required string Displayname { get; set; }
	public List<DisplaynameHistory>? DisplaynameHistory { get; set; }
	public DateTimeOffset DisplaynameChangeDate { get; set; }
	#endregion Display name

	#region TitleWords
	public string TitleWords { get; set; } = string.Empty;
	public DateTimeOffset TitleWordsChangeDate { get; set; }
	#endregion TitleWords

	#region User Roles
	public UserRoleDTO UserRole { get; set; }
	public List<UserRoleHistory>? UserRoleHistory { get; set; }
	public DateTimeOffset UserRoleChangeDate { get; set; }
	#endregion User Roles

	public DateTimeOffset CreationDate { get; set; }

	public static NonSensitiveUserDTO FromDBModel(UserID userID)
	{
		return new NonSensitiveUserDTO()
		{
			ID = userID.ID,
			Username = userID.Username,
			IconChangeDate = userID.IconChangeDate,
			Displayname = userID.Displayname,
			DisplaynameHistory = userID.DisplaynameHistory.Count > 0 ? userID.DisplaynameHistory : null,
			DisplaynameChangeDate = userID.DisplaynameChangeDate,
			TitleWords = userID.TitleWords,
			TitleWordsChangeDate = userID.TitleWordsChangeDate,
			UserRole = (UserRoleDTO)(int)userID.UserRole,
			UserRoleHistory = userID.UserRoleHistory.Count > 0 ? userID.UserRoleHistory : null,
			UserRoleChangeDate = userID.UserRoleChangeDate,
			CreationDate = userID.CreationDate
		};
	}
}
