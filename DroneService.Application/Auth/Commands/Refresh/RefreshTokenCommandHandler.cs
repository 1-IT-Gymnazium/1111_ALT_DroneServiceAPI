using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Services;
using DroneService.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly AppDbContext _dbContext;
    private readonly TokenService _tokenService;
    private readonly IClock _clock;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenCommandHandler(AppDbContext dbContext, TokenService tokenService,IClock clock, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _clock = clock;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hashedToken = _tokenService.Hash(request.RefreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken, cancellationToken);

        if (storedToken == null || storedToken.ExpiresAt < _clock.GetCurrentInstant() || storedToken.RevokedAt != null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _dbContext.Users.FindAsync(new object[] { storedToken.UserId }, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        storedToken.RevokedAt = _clock.GetCurrentInstant();

        var newAccessToken = _tokenService.GenerateAccessToken(
            user.Id, user.UserName!, user.Email!,
            expiresInMinutes: 30, 
            additionalClaims: new List<Claim>()); 

        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(
            user.Id, expirationInDays: 7, request.RequestInfo, new List<Claim>());

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var context = _httpContextAccessor.HttpContext!;
        context.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, 
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        return new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
