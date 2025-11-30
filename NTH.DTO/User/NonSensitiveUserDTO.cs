namespace NTH.DTO.User;

public partial class NonSensitiveUserDTO
{
    public long ID { get; set; }
    public required string Username { get; set; }
    #region Profile Icon
    public DateTimeOffset IconChangeDate { get; set; }
    #endregion Profile Icon

    #region Display name
    public required string Displayname { get; set; }
    public List<DisplaynameHistoryDTO>? DisplaynameHistory { get; set; }
    public DateTimeOffset DisplaynameChangeDate { get; set; }
    #endregion Display name

    #region TitleWords
    public string TitleWords { get; set; } = string.Empty;
    public DateTimeOffset TitleWordsChangeDate { get; set; }
    #endregion TitleWords

    #region User Roles
    public UserRoleDTO UserRole { get; set; }
    public List<UserRoleHistoryDTO>? UserRoleHistory { get; set; }
    public DateTimeOffset UserRoleChangeDate { get; set; }
    #endregion User Roles

    public DateTimeOffset CreationDate { get; set; }
}
