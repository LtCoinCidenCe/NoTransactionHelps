using NTH.Utilities;

namespace NTH.Models.DLPTask;

public class DLPTaskID
{
	public long ID { get; set; }
	public long ByUserAudit { get; set; }
	public required string URL { get; set; }
	public required DLPTaskStatus Status { get; set; }
	public string? ErrorMessage { get; set; } = null;
}

public enum DLPTaskStatus
{
	Enqueued = 1,
	Failed = 8,
	Warning = 16,
	Done = 32
}
