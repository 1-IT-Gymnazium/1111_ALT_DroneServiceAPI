using DroneService.Application.Auth.Commands.Login;
using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Services;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Shared.BackgroundWorkers;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using DroneService.Utilities.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NodaTime;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DroneService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1️ Database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.UseNodaTime();
            });
        });

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // 2️ Core services
        builder.Services.AddTransient<TokenService>();
        builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
        builder.Services.AddScoped<IApplicationMapper, ApplicationMapper>();

        // 3️ Identity
        builder.Services.AddIdentityCore<AppUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // 4️ JWT
        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
        var jwtSettings = builder.Configuration.GetRequiredSection(nameof(JwtSetting)).Get<JwtSetting>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

        // 5️ Options & utility services
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.Configure<EnvironmentOptions>(builder.Configuration.GetSection("EnvironmentSettings"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EnvironmentOptions>>().Value);
        builder.Services.AddSingleton<IClock>(SystemClock.Instance);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<UserContext>();

        // 6️ Background workers
        builder.Services.AddHostedService<EmailSenderBackgroundService>();

        // 7️ HttpClient & dependent services
        // DŮLEŽITÉ: Registrace HttpClient, jinak DI selže
        builder.Services.AddHttpClient<IFieldImportService, FieldImportService>();
        builder.Services.AddHttpClient<ArcGisService>();

        // 8️ MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
        });

        // 9️ Authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("role", "Admin"));
        });

        // 10 Controllers & Swagger
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token without the 'Bearer' prefix.\n\nExample: abc123xyz"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // 11️ CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .WithOrigins("http://localhost:8080")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        // 12️ Build & Run
        var app = builder.Build();

        app.UseCors("AllowFrontend");

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
