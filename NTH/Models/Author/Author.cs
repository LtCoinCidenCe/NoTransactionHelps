using System.ComponentModel.DataAnnotations;

namespace NTH.Models.Author;

public class Author
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
}
