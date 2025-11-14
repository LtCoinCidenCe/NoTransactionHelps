using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NTH.Models.Author;

namespace NTH.Models.Video;

public class VideoID
{
    public long ID { get; set; }

    #region Video itself
    [Column(name: "AuthorID"), JsonIgnore]
    public AuthorID? Author { get; set; }
    [Column(name: "AuthorID")]
    public long AuthorID { get; set; }
    [MaxLength(200)]
    public string YoutubePage { get; set; } = string.Empty;
    [MaxLength(200)]
    public string NiconicoPage { get; set; } = string.Empty;
    // If any author requests video to be translated for things here...
    [MaxLength(200)]
    public string BilibiliPage { get; set; } = string.Empty;
    public DateTimeOffset UploadDate { get; set; } =
        new DateTimeOffset(1930, 1, 1, 0, 0, 0, TimeSpan.FromHours(0)); // that's before computer came into reality
    #endregion Video itself

    // #region Our workers
    // [Column(name: "TranslatorID"), JsonIgnore]
    // public UserID? Translator { get; set; }
    // [Column(name: "TranslatorID")]
    // public long TranslatorID { get; set; }
    // [Column(name: "ScripterID"), JsonIgnore]
    // public UserID? Scripter { get; set; }
    // [Column(name: "ScripterID")]
    // public long ScripterID { get; set; }
    // [Column(name: "HardsubberID"), JsonIgnore]
    // public UserID? Hardsubber { get; set; }
    // [Column(name: "HardsubberID")]
    // public long HardsubberID { get; set; }
    // #endregion Our workers

    #region Work details
    public string TensaiRequirement { get; set; } = string.Empty;
    [MaxLength(999_9999)] // 1 qian wan
    public string TranslationText { get; set; } = string.Empty;
    #endregion Work details

    [MaxLength(200)]
    public string FinishedProductLink { get; set; } = string.Empty;
}
