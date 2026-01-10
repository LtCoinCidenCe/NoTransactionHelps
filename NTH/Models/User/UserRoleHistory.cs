using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NTH.Models.User;

[Index(nameof(CreationDate))]
public partial class UserRoleHistory
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

    public UserRoleDTO UserRole { get; set; }
}
