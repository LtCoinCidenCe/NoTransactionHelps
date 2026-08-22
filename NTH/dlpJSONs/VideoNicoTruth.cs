#pragma warning disable IDE1006 // 命名样式
using System.ComponentModel.DataAnnotations;

namespace NTH.dlpJSONs;

public class VideoNicoTruth
{
	[MaxLength(11)]
	public required string id { get; set; }
	[MaxLength(60)]
	public required string uploader { get; set; }
	[MaxLength(60)]
	public required string uploader_id { get; set; }
	[MaxLength(200)]
	public required string title { get; set; }
	[MaxLength(4000)]
	public string description { get; set; } = string.Empty;
	public int duration { get; set; } // in seconds
	public long timestamp { get; set; } // uploaded, read like Javascript new Date(timestamp*1000)
	public int comment_count { get; set; }
	public int like_count { get; set; }
	public long view_count { get; set; }
	[MaxLength(80)]
	public required string webpage_url { get; set; }
	public string? playlist_id { get; set; }
	public int width { get; set; }
	public int height { get; set; }
	public List<string> tags { get; set; } = [];
}
