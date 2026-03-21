using DroneService.Application.Contracts.Interfaces;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using DroneService.Utilities.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DroneService.Application.Contracts.Services;

// Implementace služby pro práci s JWT tokeny (access + refresh)
public class TokenService : ITokenService
{
    private readonly JwtSetting _jwtSettings; // konfigurace JWT (secret, issuer, audience...)
    private readonly IClock _clock;           // práce s časem (NodaTime)
    private readonly AppDbContext _dbContext; // databáze pro ukládání refresh tokenů

    public TokenService(IOptions<JwtSetting> jwtSettings, IClock clock, AppDbContext dbContext)
    {
        _jwtSettings = jwtSettings.Value;
        _clock = clock;
        _dbContext = dbContext;
    }

    public string GenerateAccessToken(Guid userId, string username, string email, int expiresInMinutes, IList<Claim> additionalClaims)
    {
        // základní claims, které budou v JWT tokenu
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()), // subject (ID uživatele)
            new(ClaimTypes.NameIdentifier, userId.ToString()),   // standardní claim pro identifikaci uživatele
            new(JwtRegisteredClaimNames.Email, email),           // email uživatele
            new(JwtRegisteredClaimNames.Name, username)          // jméno uživatele
        };

        // přidání dalších claims (např. role)
        if (additionalClaims != null && additionalClaims.Any())
        {
            claims.AddRange(additionalClaims);
        }

        // vytvoření podpisového klíče ze secretu
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        // nastavení podpisového algoritmu
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // vytvoření JWT tokenu
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,     // kdo token vydal
            audience: _jwtSettings.Audience, // komu je určen
            claims: claims,                 // data v tokenu
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes), // expirace
            signingCredentials: creds       // podpis
        );

        // serializace tokenu do stringu
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, int expirationInDays, string requestInfo, IList<Claim> claims)
    {
        // vytvoření náhodného tokenu (GUID jako string)
        var rawToken = Guid.NewGuid().ToString();

        // zahashování tokenu pro bezpečné uložení do DB
        var hashedToken = Hash(rawToken);

        // aktuální čas
        var now = _clock.GetCurrentInstant();

        // uložení refresh tokenu do databáze
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = hashedToken, // ukládá se hash, ne raw token
            CreatedAt = now,
            ExpiresAt = now.Plus(Duration.FromDays(expirationInDays)), // expirace
            RequestInfo = requestInfo // info o zařízení / requestu
        });

        // uložení do DB
        await _dbContext.SaveChangesAsync();

        // vrací se raw token (ten dostane klient)
        return rawToken;
    }

    public string Hash(string input)
    {
        // převod stringu na byte array
        var bytes = Encoding.UTF8.GetBytes(input);

        // vytvoření SHA256 hashe
        var hash = SHA256.HashData(bytes);

        // převod hashe na string (Base64)
        return Convert.ToBase64String(hash);
    }
}