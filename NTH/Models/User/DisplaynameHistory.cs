using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NTH.DTO.User;

namespace NTH.Models.User;

[Index(nameof(CreationDate))]
public partial class DisplaynameHistory
{
    #region itsumono
    public long ID { get; set; }

    [Column(name: "UserID")]
    public long UserID { get; set; }
    [Column(name: "UserID")]
    [JsonIgnore]
    public UserID? User { get; set; }

    public required DateTimeOffset CreationDate { get; set; }

    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;
    #endregion itsumono

    [MaxLength(30)]
    public required string Displayname { get; set; }
}

/// <summary>
/// partial class for all methods
/// </summary>
public partial class DisplaynameHistory
{
    public DisplaynameHistoryDTO toDTO()
    {
        return new DisplaynameHistoryDTO() { UserID = UserID, Displayname = Displayname, CreationDate = CreationDate };
    }
}
