using Microsoft.EntityFrameworkCore;
using NTH.DBContext;
using NTH.Models.DLPTask;
using NTH.Utilities;
using System.Diagnostics;
using System.Threading.Channels;

namespace NTH.Services;

public class YtdlpTask
{
	public long TaskID { get; set; } = 0;
	public required SiteDeVideo SiteDeVideo;
	public required TypeDeExtraction TypeDeExtraction;
	public required string SubjectID;
	public required long ByUserAudit;
	public string? URL;
}

public class YtdlpInstanceService
{
	public YtdlpInstanceService(ILogger<YtdlpInstanceService> diLogger, IServiceScopeFactory diScopeFactory)
	{
		logger = diLogger;
		scopeFactory = diScopeFactory;
		Task.Run(ProcessFirstTasksAsync);
		Task.Run(ProcessSecondTasksAsync);
	}

	public async Task EnqueueTaskAsync(YtdlpTask task) => await firstChannel.Writer.WriteAsync(task);

	public void ChannelClosing() => firstChannel.Writer.Complete();

	private async Task ProcessFirstTasksAsync()
	{
		using var scope = scopeFactory.CreateScope();
		SQLiteContext? database = scope.ServiceProvider.GetService<SQLiteContext>();
		if (database is null)
		{
			logger.LogError("Failed to resolve SQLiteContext from service provider.");
			throw new NTHException("Failed to resolve SQLiteContext for DLPTask");
		}

		await foreach (var task in firstChannel.Reader.ReadAllAsync())
		{
			try
			{
				switch (task.SiteDeVideo)
				{
					case SiteDeVideo.Undesignated:
						break;
					case SiteDeVideo.Niconico:
						switch (task.TypeDeExtraction)
						{
							case TypeDeExtraction.Undesignated:
								break;
							case TypeDeExtraction.User:
								task.URL = $"https://www.nicovideo.jp/user/{task.SubjectID}";
								break;
							case TypeDeExtraction.Video:
								break;
							default:
								break;
						}
						break;
					case SiteDeVideo.Youtube:
						break;
					case SiteDeVideo.Bilibili:
						break;
					case SiteDeVideo.X:
						break;
					default:
						break;
				}
				if (string.IsNullOrEmpty(task.URL))
				{
					logger.LogWarning("Task URL is null or empty for task: {task}", task);
					continue;
				}

				var record = new DLPTaskID { ByUserAudit = task.ByUserAudit, Status = DLPTaskStatus.Enqueued, URL = task.URL };
				await database.DLPTasks.AddAsync(record);
				await database.SaveChangesAsync();
				task.TaskID = record.ID;
				await secondChannel.Writer.WriteAsync(task);
			}
			catch (Exception ex)
			{
				logger.LogError("{ex}", ex);
			}
		}
	}

	private async Task ProcessSecondTasksAsync()
	{
		using var scope = scopeFactory.CreateScope();
		SQLiteContext? database = scope.ServiceProvider.GetService<SQLiteContext>();
		if (database is null)
		{
			logger.LogError("Failed to resolve SQLiteContext from service provider.");
			throw new NTHException("Failed to resolve SQLiteContext for DLPTask");
		}

		await foreach (var task in secondChannel.Reader.ReadAllAsync())
		{
			await TaskStation.WaitAsync();
			try
			{
				using var worker = new Process();
				worker.StartInfo.FileName = "yt-dlp";
				worker.StartInfo.Arguments = $"--write-thumbnail --write-description --write-info-json --no-download --no-cache-dir --force-overwrites {task.URL}";
				worker.StartInfo.WorkingDirectory = dlpPath;
				worker.StartInfo.RedirectStandardOutput = true;
				worker.StartInfo.RedirectStandardError = true;
				worker.Start();
				worker.WaitForExit(TimeSpan.FromMinutes(1.5));
				var sr = worker.StandardOutput.ReadToEnd();
				var se = worker.StandardError.ReadToEnd();
				logger.LogWarning(message: "{se}", se);

				if (!string.IsNullOrEmpty(se))
				{
					await database.DLPTasks.Where(x => x.ID == task.TaskID)
						.ExecuteUpdateAsync(setter =>
						setter.SetProperty(y => y.Status, DLPTaskStatus.Warning)
						.SetProperty(y => y.ErrorMessage, se));
				}
				else
				{
					await database.DLPTasks.Where(x => x.ID == task.TaskID)
						.ExecuteUpdateAsync(setter => setter.SetProperty(y => y.Status, x => x.Status == DLPTaskStatus.Warning ? x.Status : DLPTaskStatus.Done));
				}
			}
			catch (Exception ex)
			{
				await database.DLPTasks.Where(x => x.ID == task.TaskID)
					.ExecuteUpdateAsync(setter =>
					setter.SetProperty(y => y.Status, DLPTaskStatus.Failed)
					.SetProperty(y => y.ErrorMessage, ex.Message));
				throw;
			}
			TaskStation.Release();
		}
	}

	public static string dlpPath = null!;
	public static SemaphoreSlim TaskStation = new(1, 1);
	private readonly IServiceScopeFactory scopeFactory; // for resolving database instance
	private readonly ILogger<YtdlpInstanceService> logger;
	private readonly Channel<YtdlpTask> firstChannel = Channel.CreateUnbounded<YtdlpTask>();
	private readonly Channel<YtdlpTask> secondChannel = Channel.CreateUnbounded<YtdlpTask>();
}
