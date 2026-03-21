using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Services;
using DroneService.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.Refresh;

// Handler → obnoví access token pomocí refresh tokenu
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly TokenService _tokenService;
    private readonly IClock _clock;

    // Používáš HttpContext → nastavuješ cookie přímo v handleru
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenCommandHandler(
        AppDbContext dbContext,
        TokenService tokenService,
        IClock clock,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _clock = clock;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // =========================================
        // 1. HASH TOKENU (nikdy nepracujeme s plain textem)
        // =========================================
        var hashedToken = _tokenService.Hash(request.RefreshToken);

        // =========================================
        // 2. NAJÍT TOKEN V DB
        // =========================================
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken, cancellationToken);

        // Kontrola validity
        if (storedToken == null
            || storedToken.ExpiresAt < _clock.GetCurrentInstant()
            || storedToken.RevokedAt != null)
        {
            // Token je neplatný → fail
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // =========================================
        // 3. NAJÍT UŽIVATELE
        // =========================================
        var user = await _dbContext.Users
            .FindAsync(new object[] { storedToken.UserId }, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        // =========================================
        // 4. ROTACE TOKENU (VELMI DŮLEŽITÉ 🔥)
        // =========================================

        // Starý refresh token zneplatníme
        storedToken.RevokedAt = _clock.GetCurrentInstant();

        // =========================================
        // 5. VYTVOŘENÍ NOVÉHO ACCESS TOKENU (JWT)
        // =========================================
        var newAccessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.UserName!,
            user.Email!,
            expiresInMinutes: 30,
            additionalClaims: new List<Claim>() // ⚠️ tady by měly být role!
        );

        // =========================================
        // 6. VYTVOŘENÍ NOVÉHO REFRESH TOKENU
        // =========================================
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(
            user.Id,
            expirationInDays: 7,
            request.RequestInfo,
            new List<Claim>() // ⚠️ i tady můžeš přidat claims
        );

        // Uloží změny (revokace + nový token v DB)
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 7. NASTAVENÍ COOKIE
        // =========================================
        var context = _httpContextAccessor.HttpContext!;

        context.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true, // JS k tomu nemá přístup → bezpečnost
            Secure = false,  // ⚠️ v produkci MUSÍ být true!
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        // =========================================
        // 8. RESPONSE
        // =========================================
        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}