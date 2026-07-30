namespace NTH.Models.NTH;

public class ProgramStart
{
	public int ID { get; set; }
	public DateTimeOffset StartTime { get; set; } = DateTimeOffset.UtcNow;
	public bool IsFailed { get; set; } = false;
	public string? ErrorMessage { get; set; } = null;
}
