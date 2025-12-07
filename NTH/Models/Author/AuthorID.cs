using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NTH.DTO.Author;
using NTH.Models.Video;
using NTH.Models.Work;

namespace NTH.Models.Author;

[Index(nameof(Name), IsUnique = true)]
public partial class AuthorID
{
    #region Author Itself
    public long ID { get; set; }
    [MaxLength(30)]
    public required string Name { get; set; }
    #region Profile Icon
    [MaxLength(MAX_ICON_SIZE)]
    public byte[]? Icon { get; set; }
    public DateTimeOffset IconChangeDate { get; set; }
    #endregion Profile Icon
    [MaxLength(200)]
    public string YoutubeHomePage { get; set; } = string.Empty;
    [MaxLength(200)]
    public string NiconicoHomePage { get; set; } = string.Empty;
    [MaxLength(200)]
    public string BilibiliHomePage { get; set; } = string.Empty;
    [MaxLength(200)]
    public string TwitterHomePage { get; set; } = string.Empty;
    #endregion Author Itself

    #region Authorization
    public bool AuthorizedPerVideo { get; set; } = false;
    public bool AllVideoAuthorized { get; set; } = false;
    public List<AuthorizationChangeHistory> AuthorizationChangeHistory { get; set; } = new();
    public DateTimeOffset AuthorizationChangeDate { get; set; }
    /// <summary>
    /// the userID who contacts the author
    /// even though here is a List, there could only be 0 or 1 value.
    /// </summary>
    [JsonIgnore] // TODO needs to decide
    public List<WorkContact> Contact { get; set; } = new();
    #endregion Authorization

    #region TensaiRequirements
    [MaxLength(800)]
    public string AdditionalRequirements { get; set; } = string.Empty;
    public List<AdditionalRequirementsHistory> AdditionalRequirementsHistory { get; set; } = new();
    public DateTimeOffset AdditionalRequirementsChangeDate { get; set; }
    #endregion TensaiRequirements

    #region :n Video
    public List<VideoID> Videos { get; set; } = new();
    #endregion :n Video

    public DateTimeOffset CreationDate { get; set; }
    // we are not going to delete the authors anyway
    // public bool IsDeleted { get; set; } = false;
}

public partial class AuthorID
{
    public const int MAX_ICON_SIZE = 3_000_000; // 3MB
    /// <summary>
    /// be aware this returned object contains AdditionalRequirementsHistory and AuthorizationChangeHistory
    /// </summary>
    /// <param name="newAuthorDTO"></param>
    /// <returns></returns>
    public static AuthorID FromDTO(NewAuthorDTO newAuthorDTO)
    {
        var datetime = DateTimeOffset.UtcNow;
        var newAuthor = new AuthorID()
        {
            Name = newAuthorDTO.Name,
            YoutubeHomePage = newAuthorDTO.YoutubeHomePage,
            NiconicoHomePage = newAuthorDTO.NiconicoHomePage,
            BilibiliHomePage = newAuthorDTO.BilibiliHomePage,
            TwitterHomePage = newAuthorDTO.TwitterHomePage,
            AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
            AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
            AuthorizationChangeDate = datetime,
            AdditionalRequirements = newAuthorDTO.TensaiRequirement,
            AdditionalRequirementsChangeDate = datetime,
            CreationDate = datetime
        };
        newAuthor.AdditionalRequirementsHistory.Add(new()
        {
            CreationDate = datetime,
            TensaiRequirements = newAuthorDTO.TensaiRequirement
        });
        newAuthor.AuthorizationChangeHistory.Add(new()
        {
            AuthorizedPerVideo = newAuthorDTO.AuthorizedPerVideo,
            AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
            CreationDate = datetime
        });
        return newAuthor;
    }
}
