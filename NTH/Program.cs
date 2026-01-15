using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NTH.DBContext;
using NTH.Services;
using NTH.Utilities;
using NTH.Utilities.Middlewares;
using System.Text;

namespace NTH;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddResponseCaching();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("developing", builder =>
            {
                builder.AllowAnyHeader().AllowAnyMethod()
                .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
            });
        });
        builder.Services.AddSwaggerGen(options =>
        {
            var au = "Au";
            var bearer = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Description = "Enter you Bearer token here"
            };
            options.AddSecurityDefinition(au, bearer);

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference(au, document), new List<string>() }
            });
        });
        builder.Services.AddDbContext<PostgresContext>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<AuthorService>();
        builder.Services.AddScoped<SupplementaryService>();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = JwtHelper.ISSUER,
                ValidateAudience = false,
                // AudienceValidator = audval,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtHelper.SECRET)),
                ValidateLifetime = true
            };
        });
        //builder.Services.AddHangfire(config =>
        //    config.UsePostgreSqlStorage(c =>
        //    c.UseNpgsqlConnection("Host=localhost;Username=nthuser;Password=stillnicedatabase;Database=nthwork;Include Error Detail=True;")));
        //builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.UseHangfireDashboard();
            app.UseCors("developing");
        }

        app.UseHttpsRedirection();
        app.MapStaticAssets().ShortCircuit();

        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestLimiter>();
        app.UseMiddleware<HomepageGuide>();
        app.MapControllers();

        // Hangfire 0 retry
        //GlobalConfiguration.Configuration.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });

        app.Run();
    }
}
