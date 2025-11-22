using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NTH.DTO.Author;
using NTH.Models.Video;

namespace NTH.Models.Author;

[Index(nameof(Name), IsUnique = true)]
public partial class AuthorID
{
    #region Author Itself
    public long ID { get; set; }
    [MaxLength(30)]
    public required string Name { get; set; }
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
    /// </summary>
    public long? Contact { get; set; }
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
    public bool IsDeleted { get; set; } = false;
}

public partial class AuthorID
{
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
            AdditionalRequirements = newAuthorDTO.TensaiRequirement,
            AdditionalRequirementsChangeDate = datetime,
            CreationDate = datetime
        };
        newAuthor.AdditionalRequirementsHistory.Add(new AdditionalRequirementsHistory()
        {
            CreationDate = datetime,
            TensaiRequirements = newAuthorDTO.TensaiRequirement
        });
        return newAuthor;
    }
}
