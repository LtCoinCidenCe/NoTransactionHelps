using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NTH.DTO.Author;

namespace NTH.Models.Author;

[Index(nameof(Name), IsUnique = true)]
public partial class Author
{
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
    /// <summary>
    /// Actually authorized, if is per video
    /// </summary>
    public bool Contacted { get; set; } = false;
    public bool AllVideoAuthorized { get; set; } = false;

    #region TensaiRequirements
    [MaxLength(800)]
    public string TensaiRequirements { get; set; } = string.Empty;
    public List<TensaiRequirementsHistory> TensaiRequirementsHistory { get; set; } = new();
    public DateTimeOffset TensaiRequirementsChangeDate { get; set; }
    #endregion TensaiRequirements

    public DateTimeOffset CreationDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}

public partial class Author
{
    public static Author FromDTO(NewAuthorDTO newAuthorDTO)
    {
        var datetime = DateTimeOffset.UtcNow;
        var newAuthor = new Author()
        {
            Name = newAuthorDTO.Name,
            YoutubeHomePage = newAuthorDTO.YoutubeHomePage,
            NiconicoHomePage = newAuthorDTO.NiconicoHomePage,
            BilibiliHomePage = newAuthorDTO.BilibiliHomePage,
            TwitterHomePage = newAuthorDTO.TwitterHomePage,
            Contacted = newAuthorDTO.Contacted,
            AllVideoAuthorized = newAuthorDTO.AllVideoAuthorized,
            TensaiRequirements = newAuthorDTO.TensaiRequirement,
            TensaiRequirementsChangeDate = datetime,
            CreationDate = datetime
        };
        newAuthor.TensaiRequirementsHistory.Add(new TensaiRequirementsHistory()
        {
            CreationDate = datetime,
            TensaiRequirements = newAuthorDTO.TensaiRequirement
        });
        return newAuthor;
    }
}
