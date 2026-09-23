using Microsoft.EntityFrameworkCore;
using NTH.Controllers;
using NTH.DBContext;
using NTH.dlpJSONs;
using NTH.Models.Author;
using NTH.Models.DLPTask;
using NTH.Models.Video;
using NTH.Utilities;
using SixLabors.ImageSharp;
using System.Diagnostics;
using System.Text.Json;
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

public class VideoNicoProcessingInfo
{
	public required string File;
	public required string FullPath;
	public VideoNicoInfo? Info;
	public byte[] ImageBytes = [];
}

public class VideoNicoDecoded
{
	public required VideoNicoInfo Info;
	public required byte[] ImageBytes;
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
				bool hasIssue = false;

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
					hasIssue = true;
					await database.DLPTasks.Where(x => x.ID == task.TaskID)
						.ExecuteUpdateAsync(setter =>
						setter.SetProperty(y => y.Status, DLPTaskStatus.Warning)
						.SetProperty(y => y.ErrorMessage, se));
				}

				var dlpFiles = Directory.EnumerateFiles(dlpPath).Select(x => new VideoNicoProcessingInfo { File = Path.GetFileName(x), FullPath = x }).ToList();
				dlpFiles.RemoveAll(x => x.File.StartsWith("NA ["));
				foreach (var file in dlpFiles)
				{
					try
					{
						if (file.FullPath.EndsWith(".info.json", StringComparison.OrdinalIgnoreCase))
						{
							using var jsonStream = File.OpenRead(file.FullPath);
							file.Info = await JsonSerializer.DeserializeAsync<VideoNicoInfo>(jsonStream);
						}
						else if (file.FullPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
						{
							using var imageStream = File.OpenRead(file.FullPath);
							using var _temp = Image.Load(imageStream);
							file.ImageBytes = await File.ReadAllBytesAsync(file.FullPath);
						}
					}
					catch (Exception)
					{
						if (!hasIssue)
							await database.DLPTasks.Where(x => x.ID == task.TaskID)
								.ExecuteUpdateAsync(setter =>
								setter.SetProperty(y => y.Status, DLPTaskStatus.Warning)
								.SetProperty(y => y.ErrorMessage, $"File {file.File} not loaded correctly."));
						hasIssue = true;
					}
				}
				var groupedFiles = dlpFiles.GroupBy(x => x.File.Substring(0, 10)).ToList();

				var videos = new List<VideoNicoDecoded>();

				foreach (var group in groupedFiles)
				{
					VideoNicoInfo? info = null;
					byte[] thumbnail = [];
					foreach (var file in group)
					{
						if (file.Info is not null)
							info = file.Info;
						if (file.ImageBytes.Length > 4)
							thumbnail = file.ImageBytes;
					}
					if (info is not null && thumbnail.Length > 4)
					{
						videos.Add(new VideoNicoDecoded { Info = info, ImageBytes = thumbnail });
						continue;
					}
					if (!hasIssue)
						await database.DLPTasks.Where(x => x.ID == task.TaskID)
							.ExecuteUpdateAsync(setter =>
							setter.SetProperty(y => y.Status, DLPTaskStatus.Warning)
							.SetProperty(y => y.ErrorMessage, $"Missing info or thumbnail: {group.Key}"));
					hasIssue = true;
				}

				if (!videos.All(x => x.Info.uploader_id == task.SubjectID))
					throw new NTHException($"Uploader ID mismatch. Expected: {task.SubjectID}, Found: {string.Join(", ", videos.Select(x => x.Info.uploader_id).Distinct())}");

				long subjectLong = long.Parse(task.SubjectID);
				AuthorID? author = await database.Authors.AsNoTracking().FirstOrDefaultAsync(x => x.NiconicoID == subjectLong);
				if (author is null)
				{
					DateTimeOffset creationDate = DateTimeOffset.UtcNow;
					author = new AuthorID()
					{
						ByUserAudit = task.ByUserAudit,
						Name = videos.First().Info.uploader,
						NiconicoID = subjectLong,
						CreationDate = creationDate,
						UpdatedAt = creationDate
					};
					database.Authors.Add(author);
					await database.SaveChangesAsync();
				}
				foreach (var video in videos)
				{
					string videoID = video.Info.id;
					VideoID? found = database.Videos.FirstOrDefault(x => x.NiconicoID == videoID);
					if (found is null)
					{
						DateTimeOffset creationDate = DateTimeOffset.UtcNow;
						var newVideo = new VideoID()
						{
							ByUserAudit = task.ByUserAudit,
							Title = video.Info.title,
							Introduction = video.Info.description,
							Tags = video.Info.tags,
							AuthorID = author.ID,
							Duration = video.Info.duration,
							CommentCount = video.Info.comment_count,
							ViewCount = video.Info.view_count,
							Like_Count = video.Info.like_count,
							NiconicoID = videoID,
							UploadDate = DateTimeOffset.FromUnixTimeSeconds(video.Info.timestamp),
							CreationDate = creationDate,
							UpdatedAt = creationDate
						};
						
						Guid guid = Guid.CreateVersion7();
						var savedPath = Path.Join(VideoCookieAssetController.VideoIconPath, guid.ToString() + ".jpg");
						await File.WriteAllBytesAsync(savedPath, video.ImageBytes);
						newVideo.ThumbnailGUID = guid;
						newVideo.ThumbnailChangeDate = creationDate;

						database.Videos.Add(newVideo);
						await database.SaveChangesAsync();
					}
				}

				if (!hasIssue)
				{
					await database.DLPTasks.Where(x => x.ID == task.TaskID)
						.ExecuteUpdateAsync(setter => setter.SetProperty(y => y.Status, x => x.Status == DLPTaskStatus.Warning ? x.Status : DLPTaskStatus.Done));
				}
			}
			catch (Exception ex) // task level failure
			{
				await database.DLPTasks.Where(x => x.ID == task.TaskID)
					.ExecuteUpdateAsync(setter =>
					setter.SetProperty(y => y.Status, DLPTaskStatus.Failed)
					.SetProperty(y => y.ErrorMessage, ex.Message));
			}
			finally
			{
				TaskStation.Release();
			}
		}
	}

	public static string dlpPath = null!;
	public static SemaphoreSlim TaskStation = new(1, 1);
	private readonly IServiceScopeFactory scopeFactory; // for resolving database instance
	private readonly ILogger<YtdlpInstanceService> logger;
	private readonly Channel<YtdlpTask> firstChannel = Channel.CreateUnbounded<YtdlpTask>();
	private readonly Channel<YtdlpTask> secondChannel = Channel.CreateUnbounded<YtdlpTask>();
}
