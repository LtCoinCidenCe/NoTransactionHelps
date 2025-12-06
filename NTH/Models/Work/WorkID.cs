using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NTH.Models.User;
using NTH.Models.Video;

namespace NTH.Models.Work;

[Index(nameof(IsFinished))]
[Index(nameof(FinishingDate))]
public class WorkID
{
    public long ID { get; set; }

    public WorkType WorkType { get; set; }

    /// <summary>
    /// Basically a computed result of video's Status*
    /// To not display the user's outdated work
    /// </summary>
    public bool IsFinished { get; set; }

    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// When the video is ready
    /// </summary>
    public DateTimeOffset FinishingDate { get; set; } = new DateTimeOffset(2495, 1, 1, 0, 0, 0, TimeSpan.Zero);

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
