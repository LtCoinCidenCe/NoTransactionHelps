using Hangfire;
using Hangfire.PostgreSql;
using NTH.DBContext;
using NTH.Scheduling;
using NTH.Services;
using NTH.Utilities.Middlewares;

namespace NTH;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<PostgresContext>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<SupplementaryService>();
        builder.Services.AddHangfire(config =>
            config.UsePostgreSqlStorage(c =>
            c.UseNpgsqlConnection("Host=localhost;Username=nthuser;Password=stillnicedatabase;Database=nthwork;Include Error Detail=True;")));
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHangfireDashboard();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<RequestLimiter>();
        app.MapControllers();

        GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
        BackgroundJob.Schedule(
            () => SchedulingTasks.ThrowException(),
            TimeSpan.FromSeconds(15)
        );

        app.Run();
    }
}
