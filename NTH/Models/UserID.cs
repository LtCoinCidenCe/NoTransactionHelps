using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models;

[Index(nameof(Username), IsUnique = true)]
public class UserID
{
    public long ID { get; set; }

    // [Length(3, 30)] this doesn't work
    [MaxLength(30)]
    public required string Username { get; set; }

    #region Display name
    // [Length(3, 30)] this doesn't work
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

    public DateTimeOffset CreationDate { get; set; }

    public bool IsDeleted { get; set; } = false;
}
