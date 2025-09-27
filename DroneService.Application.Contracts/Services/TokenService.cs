using DroneService.Application.Contracts.Interfaces;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using DroneService.Utilities.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Services;

public class TokenService : ITokenService
{
    private readonly JwtSetting _jwtSettings;
    private readonly IClock _clock;
    private readonly AppDbContext _dbContext;

    public TokenService(IOptions<JwtSetting> jwtSettings, IClock clock, AppDbContext dbContext)
    {
        _jwtSettings = jwtSettings.Value;
        _clock = clock;
        _dbContext = dbContext;
    }

    public string GenerateAccessToken(Guid userId, string username, string email, int expiresInMinutes, IList<Claim> additionalClaims)
    {
    var claims = new List<Claim>
    {
    new(JwtRegisteredClaimNames.Sub, userId.ToString()),
    new(ClaimTypes.NameIdentifier, userId.ToString()), 
    new(JwtRegisteredClaimNames.Email, email),
    new(JwtRegisteredClaimNames.Name, username)
    };


        if (additionalClaims != null && additionalClaims.Any())
        {
            claims.AddRange(additionalClaims);
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, int expirationInDays, string requestInfo, IList<Claim> claims)
    {
        var rawToken = Guid.NewGuid().ToString();
        var hashedToken = Hash(rawToken);
        var now = _clock.GetCurrentInstant();

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = hashedToken,
            CreatedAt = now,
            ExpiresAt = now.Plus(Duration.FromDays(expirationInDays)),
            RequestInfo = requestInfo
        });

        await _dbContext.SaveChangesAsync();
        return rawToken; 
    }

    public string Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
