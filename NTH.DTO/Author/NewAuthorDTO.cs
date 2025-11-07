using System.ComponentModel.DataAnnotations;

namespace NTH.DTO.Author;

public class NewAuthorDTO
{
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
    [MaxLength(800)]
    public string TensaiRequirement { get; set; } = string.Empty;
}
