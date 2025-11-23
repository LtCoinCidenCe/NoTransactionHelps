using Microsoft.EntityFrameworkCore;
using NTH.Models.Author;
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
    public DbSet<DisplaynameHistory> UserDisplaynameHistories { get; set; }
    public DbSet<UserRoleHistory> UserRoleHistories { get; set; }
    public DbSet<AuthorID> Authors { get; set; }
    public DbSet<AuthorizationChangeHistory> AuthorizationChangeHistories { get; set; }
    public DbSet<AdditionalRequirementsHistory> AdditionalRequirementsHistories { get; set; }
    public DbSet<VideoID> Videos { get; set; }
    public DbSet<WorkTranslation> WorkTranslations { get; set; }

    #region Supplementary Definition Reference Tables
    public DbSet<UserRoleSupplementary> UserRoleSupplementary { get; set; }
    #endregion Supplementary Definition Reference Tables
}
