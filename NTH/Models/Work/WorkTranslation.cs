using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NTH.Models.User;
using NTH.Models.Video;

namespace NTH.Models.Work;

public class WorkTranslation
{
    public long ID { get; set; }

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
