using NTH.DBContext;
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<RequestLimiter>();
        app.MapControllers();

        app.Run();
    }
}
