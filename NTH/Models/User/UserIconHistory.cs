using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.User;

[Index(nameof(CreationDate))]
public class UserIconHistory
{
    public const int MAX_ICON_SIZE = 3_000_000; // 3MB

    #region itsumono
    public long ID { get; set; }

    [Column(name: "UserID")]
    public long UserID { get; set; }
    [Column(name: "UserID")]
    [JsonIgnore]
    public UserID? User { get; set; }

    public required DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;
    #endregion itsumono

    [MaxLength(MAX_ICON_SIZE)]
    public byte[] Icon { get; set; } = new byte[1];
}
