using Microsoft.EntityFrameworkCore;
using NTH.Models.Author;
using NTH.Models.CharacterReality;
using NTH.Models.LiaoTian;
using NTH.Models.User;
using NTH.Models.Video;
using NTH.Models.Work;
using NTH.Utilities;

namespace NTH.DBContext;

public class SQLiteContext : DbContext
{
	private ILogger<SQLiteContext> logger;
	private IConfiguration configuration;
	private string NTHDataPath;
	public SQLiteContext(DbContextOptions<SQLiteContext> options,
	ILogger<SQLiteContext> diLogger,
	IConfiguration diConfiguration) : base(options)
	{
		logger = diLogger;
		configuration = diConfiguration;
		NTHDataPath = configuration["NTHDataPath"] ?? throw new NTHException("NTHDataPath configuration is not provided");
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite($"Data Source={NTHDataPath}/NTHdatabase.db")
		.EnableSensitiveDataLogging();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// no cascading by default, all foreign key prevents deletion
		foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
		{
			foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
		}
	}

	public DbSet<UserID> Users { get; set; }
	public DbSet<UserIconHistory> UserIconHistories { get; set; }
	public DbSet<DisplaynameHistory> UserDisplaynameHistories { get; set; }
	public DbSet<UserRoleHistory> UserRoleHistories { get; set; }
	public DbSet<WorkContact> WorkContacts { get; set; }
	public DbSet<AuthorID> Authors { get; set; }
	public DbSet<AuthorIconHistory> AuthorIconHistories { get; set; }
	public DbSet<AuthorizationChangeHistory> AuthorizationChangeHistories { get; set; }
	public DbSet<AdditionalRequirementsHistory> AdditionalRequirementsHistories { get; set; }
	public DbSet<VideoID> Videos { get; set; }
	public DbSet<WorkID> Works { get; set; }

	public DbSet<Message> LiaoTianJiLu { get; set; }

	#region World Reality Information
	// 这边数据库希望只记录增加的项，已知的尽可能代码里写出
	public DbSet<CharacterID> CharacterReality { get; set; }
	//public DbSet<Province> ChinaProvinceReality { get; set; } // 这个比较稳定还是不需要了
	#endregion World Reality Information

	#region Supplementary Definition Reference Tables
	public DbSet<UserRoleSupplementary> UserRoleSupplementary { get; set; }
	public DbSet<WorkStatusSupplementary> WorkStatusSupplementary { get; set; }
	public DbSet<WorkTypeSupplementary> WorkTypeSupplementary { get; set; }
	#endregion Supplementary Definition Reference Tables
}
