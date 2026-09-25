using Microsoft.EntityFrameworkCore;
using NTH.Models.Video;
using NTH.Models.Work;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NTH.Models.Author;

[Index(nameof(Name), IsUnique = true)]
public partial class AuthorID
{
	#region Author Itself
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	[MaxLength(30)]
	public required string Name { get; set; }
	public Guid AuthorIconID { get; set; }
	public DateTimeOffset IconChangeDate { get; set; }
	[MaxLength(200)]
	public string YoutubeHomePage { get; set; } = string.Empty;
	public long NiconicoID { get; set; }
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
	/// List works like a history table, get the one with new biggest ID
	/// </summary>
	[JsonIgnore]
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

	public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
	// we are not going to delete the authors anyway
	// public bool IsDeleted { get; set; } = false;

	#region Calculated Properties
	public string NiconicoHomePage { get => $"https://www.nicovideo.jp/user/{NiconicoID}"; }
	#endregion Calculated Properties
}
