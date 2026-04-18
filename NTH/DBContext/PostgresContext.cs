using Microsoft.EntityFrameworkCore;
using NTH.Models.Author;
using NTH.Models.CharacterReality;
using NTH.Models.GeographicPlaceReality;
using NTH.Models.User;
using NTH.Models.Video;
using NTH.Models.Work;

namespace NTH.DBContext;

public class PostgresContext : DbContext
{
	private ILogger<PostgresContext> logger;
	private IConfiguration configuration;

	public PostgresContext(DbContextOptions<PostgresContext> options,
	ILogger<PostgresContext> diLogger,
	IConfiguration diConfiguration)
	: base(options)
	{
		logger = diLogger;
		configuration = diConfiguration;
		logger.Log(LogLevel.Information, "PostgresContext constructor");
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseNpgsql("Host=localhost;Username=nthuser;Password=stillnicedatabase;Database=nthwork")
			.EnableSensitiveDataLogging();
		base.OnConfiguring(optionsBuilder);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// this is useful
		modelBuilder.Entity<UserID>(entity =>
		{
			var props = typeof(UserID).GetProperties();
			foreach (var prop in props)
			{
				if (prop.Name.EndsWith("Date"))
				{
					entity.Property(prop.Name).HasDefaultValueSql("transaction_timestamp()");
				}
			}
		});
		base.OnModelCreating(modelBuilder);
	}

	public DbSet<UserID> Users { get; set; }
	public DbSet<UserIconHistory> UserIconHistories { get; set; }
	public DbSet<DisplaynameHistory> UserDisplaynameHistories { get; set; }
	public DbSet<UserRoleHistory> UserRoleHistories { get; set; }
	public DbSet<WorkContact> WorkContacts { get; set; }
	public DbSet<AuthorID> Authors { get; set; }
	public DbSet<AuthorizationChangeHistory> AuthorizationChangeHistories { get; set; }
	public DbSet<AdditionalRequirementsHistory> AdditionalRequirementsHistories { get; set; }
	public DbSet<VideoID> Videos { get; set; }
	public DbSet<WorkID> Works { get; set; }

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
