namespace NTH.Models.DLPTask;

public class DLPTaskID
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	public required string URL { get; set; }
	public required DLPTaskStatus Status { get; set; }
	public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
	public string? ErrorMessage { get; set; } = null;
}
