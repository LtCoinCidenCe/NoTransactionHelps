using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
    [MaxLength(MAX_ICON_SIZE)]
    [JsonIgnore]
    public byte[]? Icon { get; set; }
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
    public List<WorkTranslation> WorkTranslations { get; set; } = new();
    #endregion AllWorks

    public DateTimeOffset CreationDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// partial class for all methods
/// </summary>
public partial class UserID
{
    public const int MAX_ICON_SIZE = 3_000_000; // 3MB
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
