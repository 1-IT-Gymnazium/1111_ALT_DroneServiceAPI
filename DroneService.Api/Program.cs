using DroneService.Application.Auth.Commands.Login;
using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Services;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Shared.BackgroundWorkers;
using NodaTime.Serialization.SystemTextJson;
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
        // Vytvoření builderu - základ celé ASP.NET aplikace
        var builder = WebApplication.CreateBuilder(args);

        // =========================================
        // 1️ DATABASE (PostgreSQL + NodaTime)
        // =========================================
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            // Používáme PostgreSQL
            options.UseNpgsql(connectionString, npgsql =>
            {
                // NodaTime = lepší práce s datem/časem než klasický DateTime
                npgsql.UseNodaTime();
            });
        });

        // Tohle je důležité:
        // ASP.NET si jinak automaticky mapuje claimy (např. "sub" → "nameidentifier")
        // což může rozbít JWT logiku → proto to vypínáme
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // =========================================
        // 2️ CORE SERVICES (Dependency Injection)
        // =========================================

        // TokenService – generuje JWT tokeny
        builder.Services.AddTransient<TokenService>();

        // Email service – posílání emailů
        builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

        // Mapper – převádí entity ↔ DTO
        builder.Services.AddScoped<IApplicationMapper, ApplicationMapper>();

        // =========================================
        // 3️ IDENTITY (uživatelé, hesla, login)
        // =========================================
        builder.Services.AddIdentityCore<AppUser>(options =>
        {
            // Uživatel se musí potvrdit (např. přes email)
            options.SignIn.RequireConfirmedAccount = true;

            // Nastavení pravidel pro hesla
            options.Password.RequireDigit = true; // musí obsahovat číslo
            options.Password.RequireLowercase = true; // malé písmeno
            options.Password.RequireNonAlphanumeric = true; // speciální znak
            options.Password.RequireUppercase = true; // velké písmeno
            options.Password.RequiredLength = 6; // minimální délka
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AppDbContext>() // ukládání uživatelů do DB
        .AddSignInManager() // login logika
        .AddDefaultTokenProviders(); // tokeny pro reset hesla apod.

        // =========================================
        // 4️ JWT AUTHENTICATION
        // =========================================

        // Načtení nastavení z appsettings.json (Issuer, Audience, SecretKey)
        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection(nameof(JwtSetting)));
        var jwtSettings = builder.Configuration.GetRequiredSection(nameof(JwtSetting)).Get<JwtSetting>();

        builder.Services.AddAuthentication(options =>
        {
            // Nastavíme JWT jako defaultní autentizaci
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Tady říkáme jak se má token validovat
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, // kontrola kdo token vydal
                ValidateAudience = true, // pro koho je token určen
                ValidateLifetime = true, // jestli neexpiruje
                ValidateIssuerSigningKey = true, // kontrola podpisu

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                // Secret key → používá se na podpis tokenu
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                )
            };
        });

        // =========================================
        // 5️ OPTIONS & UTILITY SERVICES
        // =========================================

        // SMTP konfigurace (emaily)
        builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpSettings"));

        // Environment settings (např. jestli běžíš dev/prod)
        builder.Services.Configure<EnvironmentOptions>(builder.Configuration.GetSection("EnvironmentSettings"));

        // Zpřístupnění EnvironmentOptions přímo (bez IOptions<>)
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<EnvironmentOptions>>().Value);

        // IClock = NodaTime interface pro práci s časem (lepší testovatelnost)
        builder.Services.AddSingleton<IClock>(SystemClock.Instance);

        // Umožňuje přístup k HttpContext odkudkoliv
        builder.Services.AddHttpContextAccessor();

        // UserContext = vlastní service pro získání aktuálního uživatele (např. z JWT)
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<UserContext>();

        // =========================================
        // 6️ BACKGROUND WORKERS
        // =========================================

        // Service běžící na pozadí (např. odesílání emailů z fronty)
        builder.Services.AddHostedService<EmailSenderBackgroundService>();

        // =========================================
        // 7️ HTTP CLIENTS
        // =========================================

        // Důležité: bez toho by DI nefungovalo pro služby co používají HttpClient
        builder.Services.AddHttpClient<IFieldImportService, FieldImportService>();

        // ArcGis service (pravděpodobně pro mapy)
        builder.Services.AddHttpClient<ArcGisService>();

        // =========================================
        // 8️ MEDIATR (CQS architektura)
        // =========================================

        builder.Services.AddMediatR(cfg =>
        {
            // Zaregistruje všechny Command/Query handlery
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
        });

        // =========================================
        // 9️ AUTHORIZATION (role, policy)
        // =========================================

        builder.Services.AddAuthorization(options =>
        {
            // Vlastní policy → pouze admin může přistupovat
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("role", "Admin"));
        });

        // =========================================
        // 10️ CONTROLLERS + JSON + SWAGGER
        // =========================================

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                // Přidání podpory pro NodaTime v JSONu
                options.JsonSerializerOptions.ConfigureForNodaTime(
                    DateTimeZoneProviders.Tzdb
                );
            });

        // Swagger = dokumentace API
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JWT API",
                Version = "v1"
            });

            // Nastavení JWT ve Swaggeru
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Sem vlož JWT token bez 'Bearer' prefixu"
            });

            // Řekne Swaggeru, že endpointy můžou vyžadovat token
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

        // =========================================
        // 11️ CORS (frontend komunikace)
        // =========================================

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .WithOrigins("http://localhost:8080") // Vue frontend
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        // =========================================
        // 12️ BUILD & RUN APP
        // =========================================

        var app = builder.Build();

        // Povolení CORS
        app.UseCors("AllowFrontend");

        // Swagger jen v developmentu
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Middleware pipeline
        app.UseHttpsRedirection(); // přesměrování na HTTPS
        app.UseAuthentication();   // ověřování uživatele (JWT)
        app.UseAuthorization();    // kontrola oprávnění

        app.MapControllers(); // napojení controllerů

        app.Run(); // spuštění aplikace
    }
}