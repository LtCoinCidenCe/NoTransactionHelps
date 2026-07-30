using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NTH.DBContext;
using NTH.Middlewares;
using NTH.Services;
using NTH.SignalRHubs;
using NTH.Utilities;
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
				builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
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
		builder.Services.AddDbContext<SQLiteContext>();
		builder.Services.AddScoped<UserService>();
		builder.Services.AddScoped<AuthorService>();
		builder.Services.AddScoped<SupplementaryService>();
		builder.Services.AddScoped<RequestingUser>();
		builder.Services.AddAuthentication(options =>
		{
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddCookie(options =>
		{
			options.Cookie.Name = "NTHCookie";
			options.Cookie.HttpOnly = true;
			options.Cookie.SameSite = SameSiteMode.Strict;
			options.ExpireTimeSpan = TimeSpan.FromMinutes(16);
			options.Events = new CookieAuthenticationEvents
			{
				OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; },
				OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; }
			};
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
			// for signalR
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var accessToken = context.Request.Query["access_token"];

					// If the request is for our hub...
					var path = context.HttpContext.Request.Path;
					if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/LiaoTianHub"))
					{
						// Read the token out of the query string
						context.Token = accessToken;
					}
					return Task.CompletedTask;
				}
			};
		});
		builder.Services.AddSignalR();
		//builder.Services.AddHangfire(config =>
		//    config.UsePostgreSqlStorage(c =>
		//    c.UseNpgsqlConnection("Host=localhost;Username=nthuser;Password=stillnicedatabase;Database=nthwork;Include Error Detail=True;")));
		//builder.Services.AddHangfireServer();

		app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			//app.UseHangfireDashboard();
			app.UseCors("developing");
		}
		else if (app.Environment.IsStaging())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.MapStaticAssets().ShortCircuit();

		app.UseResponseCaching();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapHub<LiaoTianHub>("/api/LiaoTianHub").RequireAuthorization();

		app.UseMiddleware<HomepageGuide>();
		app.UseMiddleware<UserExtractor>();
		app.MapControllers();

		var configuration = app.Services.GetService<IConfiguration>() ?? throw new NTHException("Why IConfiguration is null???");
		var nthDataPath = configuration.GetValue<string>("NTHDataPath") ?? throw new NTHException("You need to provide a valid NTHDataPath in appsettings.json");

		app.Lifetime.ApplicationStarted.Register(() =>
		{
			var server = app.Services.GetRequiredService<IServer>();
			var addresses = server.Features.GetRequiredFeature<IServerAddressesFeature>();
			// exception does not stop the program here.
			if (addresses.Addresses.Count == 0)
				throw new NTHException("No addresses in server features.");
			string firstAddress = addresses.Addresses.First();
			string statedURL = $"{firstAddress}/api/Ping/Started";
			using var http = new HttpClient();
			if (!http.GetAsync(statedURL).Wait(6000))
				throw new NTHException("Failed to ping the started URL.");
		});

		app.Start();
		app.WaitForShutdown();
	}

	public static WebApplication app = null!; // just small assurance grammar
}
