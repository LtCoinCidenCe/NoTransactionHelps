using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NTH.Models.LiaoTian;

[Index(nameof(ReceivedTime))]
public class Message
{
	public const int MaxLength = 400;
	public long ID { get; set; }
	public long UserID { get; set; }
	public DateTimeOffset ReceivedTime { get; set; } = DateTimeOffset.UtcNow;
	[MaxLength(MaxLength)]
	public required string Words { get; set; }
	public bool Revoked { get; set; } = false;
}
