using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using DroneService.Application.Contracts.Services;
using DroneService.Data.Entities.Identity;
using DroneService.Utilities.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DroneService.Application.Auth.Commands.Login;

// Handler pro login → vrací Result<LoginResponse>
public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtSetting _jwtSettings;
    private readonly TokenService _tokenService;

    public LoginHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IOptions<JwtSetting> jwtSettings,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;

        // Načtení JWT nastavení z appsettings (expiration, secret, atd.)
        _jwtSettings = jwtSettings.Value;

        // Service, která generuje access + refresh tokeny
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Normalizace emailu (Identity ukládá uppercase)
        var normalizedEmail = request.Email.ToUpperInvariant();

        // Najdeme uživatele:
        // - musí mít potvrzený email
        // - porovnáváme NormalizedUserName (ne Email!)
        // - Include → načteme i ServiceGoals (potřebujeme později)
        var user = await _userManager.Users
            .Include(u => u.ServiceGoals)
            .SingleOrDefaultAsync(x =>
                x.EmailConfirmed && x.NormalizedUserName == normalizedEmail);

        // Pokud uživatel neexistuje → fail (neprozrazujeme důvod kvůli bezpečnosti)
        if (user == null)
            return Result<LoginResponse>.Fail("LOGIN_FAILED");

        // Ověření hesla přes Identity
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (!result.Succeeded)
            return Result<LoginResponse>.Fail("LOGIN_FAILED");

        // Načteme claimy (např. role)
        var claims = await _userManager.GetClaimsAsync(user);

        // =========================================
        // GENEROVÁNÍ ACCESS TOKENU (JWT)
        // =========================================

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.UserName!,
            request.Email,
            _jwtSettings.AccessTokenExpirationInMinutes,
            claims);

        // =========================================
        // GENEROVÁNÍ REFRESH TOKENU
        // =========================================

        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(
            user.Id,
            _jwtSettings.RefreshTokenExpirationInDays,
            "", // request info (tady zatím prázdné)
            claims);

        // =========================================
        // KONTROLA, JESTLI MÁ UŽIVATEL DOPLNĚNÝ PROFIL
        // =========================================

        var requiresProfileCompletion =
            string.IsNullOrWhiteSpace(user.AgencyName)
            || string.IsNullOrWhiteSpace(user.ContactPerson)
            || string.IsNullOrWhiteSpace(user.AgencyAddress)
            || string.IsNullOrWhiteSpace(user.Ico)
            || user.ServiceGoals == null
            || !user.ServiceGoals.Any();

        // =========================================
        // RESPONSE
        // =========================================

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RequiresProfileCompletion = requiresProfileCompletion
        };

        return Result<LoginResponse>.Ok(response);
    }
}