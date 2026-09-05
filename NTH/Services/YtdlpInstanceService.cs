using NTH.DBContext;
using NTH.Utilities;
using System.Diagnostics;
using System.Threading.Channels;

namespace NTH.Services;

public class YtdlpTask
{
	public required SiteDeVideo Site;
	public required TypeDeExtraction TypeDeExtraction;
	public required string ID;
	public required long ByUserAudit;
}

public class YtdlpInstanceService
{
	public YtdlpInstanceService(ILogger<YtdlpInstanceService> diLogger, IServiceScopeFactory diScopeFactory)
	{
		logger = diLogger;
		scopeFactory = diScopeFactory;
		Task.Run(ProcessTasksAsync);
	}

	public async Task EnqueueTaskAsync(YtdlpTask task) => await taskChannel.Writer.WriteAsync(task);

	public void ChannelClosing() => taskChannel.Writer.Complete();

	private async Task ProcessTasksAsync()
	{
		using var scope = scopeFactory.CreateScope();
		var database = scope.ServiceProvider.GetService<SQLiteContext>();

		await foreach (var task in taskChannel.Reader.ReadAllAsync())
		{
			await TaskStation.WaitAsync();
			try
			{
				using var worker = new Process();
				worker.StartInfo.FileName = "yt-dlp";
				worker.StartInfo.Arguments = $"--write-thumbnail --write-description --write-info-json --no-download --no-cache-dir --force-overwrites https://www.nicovideo.jp/user/{task.ID}";
				worker.StartInfo.WorkingDirectory = Program.dlpPath;
				worker.StartInfo.RedirectStandardOutput = true;
				worker.StartInfo.RedirectStandardError = true;
				worker.Start();
				worker.WaitForExit(TimeSpan.FromMinutes(1.5));
				var sr = worker.StandardOutput.ReadToEnd();
				var se = worker.StandardError.ReadToEnd();
				logger.LogWarning(message: "{se}", se);
			}
			catch (Exception)
			{
				throw;
			}
			TaskStation.Release();
		}
	}

	public static SemaphoreSlim TaskStation = new(1, 1);
	private readonly IServiceScopeFactory scopeFactory; // for resolving database instance
	private readonly ILogger<YtdlpInstanceService> logger;
	private readonly Channel<YtdlpTask> taskChannel = Channel.CreateUnbounded<YtdlpTask>();
}
