using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NTH.Models.User;
using NTH.Models.Video;

namespace NTH.Models.Work;

[Index(nameof(UserID), nameof(WorkType))]
public class WorkID
{
    public long ID { get; set; }

    public WorkType WorkType { get; set; }

    public bool IsFinished { get; set; }

    public DateTimeOffset ChangeDate { get; set; } = DateTimeOffset.UtcNow;

    #region User
    [Column(name: "UserID")]
    public long UserID { get; set; }
    [Column(name: "UserID")]
    [JsonIgnore]
    public UserID? User { get; set; }
    #endregion User

    #region Video
    [Column(name: "VideoID")]
    public long VideoID { get; set; }
    [Column(name: "VideoID")]
    public VideoID? Video { get; set; }
    #endregion Video
}
