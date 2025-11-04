using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NTH.DBContext;
using NTH.Services;
using NTH.Utilities;
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
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Au", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description = "Enter you Bearer token here"
            });

            /* OpenApiSecurityRequirement : Dictionary<OpenApiSecurityScheme, IList<string>> */
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Au"
                        }
                    },
                    new List<string>()
                }
            });
        });
        builder.Services.AddDbContext<PostgresContext>();
        builder.Services.AddScoped<UserService>();
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestLimiter>();
        app.MapControllers();

        app.Run();
    }

    private static bool audval(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        throw new NotImplementedException();
    }
}
