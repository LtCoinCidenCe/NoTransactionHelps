using Microsoft.EntityFrameworkCore;
using NTH.Models;

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
        // this is doing nothing.
        modelBuilder.Entity<UserID>(entity =>
        {
            entity.Property(e => e.CreationDate).HasDefaultValueSql("transaction_timestamp()");
            entity.Property(e => e.DisplaynameChangeDate).HasDefaultValueSql("transaction_timestamp()");
            entity.Property(e => e.TitleWordsChangeDate).HasDefaultValueSql("transaction_timestamp()");
            entity.Property(e => e.PasswordChangeDate).HasDefaultValueSql("transaction_timestamp()");
        });
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserID> Users { get; set; }
    public DbSet<DisplaynameHistory> DisplaynameHistories { get; set; }
}
