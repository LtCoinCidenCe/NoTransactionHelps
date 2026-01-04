using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NTH.DTO.User;
using NTH.Models.Work;

namespace NTH.Models.User;

[Index(nameof(Username), IsUnique = true)]
public partial class UserID
{
    public long ID { get; set; }

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
    public long UserIconID { get; set; }
    public DateTimeOffset IconChangeDate { get; set; }
    #endregion Profile Icon

    #region Display name
    // [Length(2, 30)] this doesn't work for DB
    [MaxLength(30)]
    public required string Displayname { get; set; }
    public List<DisplaynameHistory> DisplaynameHistory { get; set; } = new();
    public DateTimeOffset DisplaynameChangeDate { get; set; }
    #endregion Display name

    #region TitleWords
    [MaxLength(250)]
    public string TitleWords { get; set; } = string.Empty;
    public DateTimeOffset TitleWordsChangeDate { get; set; }
    #endregion TitleWords

    #region Password
    [MaxLength(32)]
    public required byte[] Password { get; set; }
    [MaxLength(5)]
    public string PassSalt { get; set; } = "     ";
    public DateTimeOffset PasswordChangeDate { get; set; }
    #endregion Password

    #region User Roles
    public UserRoleDTO UserRole { get; set; }
    public List<UserRoleHistory> UserRoleHistory { get; set; } = new();
    public DateTimeOffset UserRoleChangeDate { get; set; }
    #endregion User Roles

    #region AllWorks
    public List<WorkContact> Contact { get; set; } = new();
    public List<WorkID> Works { get; set; } = new();
    #endregion AllWorks

    public DateTimeOffset CreationDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// partial class for all methods
/// </summary>
public partial class UserID
{
    public NonSensitiveUserDTO ToDTO()
    {
        return new NonSensitiveUserDTO()
        {
            ID = ID,
            Username = Username,
            IconChangeDate = IconChangeDate,
            Displayname = Displayname,
            DisplaynameHistory = DisplaynameHistory.Count > 0 ? DisplaynameHistory.Select(x => x.toDTO()).ToList() : null,
            DisplaynameChangeDate = DisplaynameChangeDate,
            TitleWords = TitleWords,
            TitleWordsChangeDate = TitleWordsChangeDate,
            UserRole = (UserRoleDTO)(int)UserRole,
            UserRoleHistory = UserRoleHistory.Count > 0 ? UserRoleHistory.Select(x => x.toDTO()).ToList() : null,
            UserRoleChangeDate = UserRoleChangeDate,
            CreationDate = CreationDate
        };
    }
}
