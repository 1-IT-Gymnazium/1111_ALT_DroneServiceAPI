using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Services;
using DroneService.Data.Entities.Identity;
using DroneService.Utilities.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DroneService.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtSetting _jwtSettings;
    private readonly TokenService _tokenService;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IOptions<JwtSetting> jwtSettings,
        TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await _userManager.Users
    .Include(u => u.ServiceGoals)
    .SingleOrDefaultAsync(x =>
        x.EmailConfirmed && x.NormalizedUserName == normalizedEmail);


        if (user == null)
            throw new Exception("LOGIN_FAILED");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!result.Succeeded)
            throw new Exception("LOGIN_FAILED");

        var claims = await _userManager.GetClaimsAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.UserName!,
            request.Email,
            _jwtSettings.AccessTokenExpirationInMinutes,
            claims);

        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(
            user.Id,
            _jwtSettings.RefreshTokenExpirationInDays,
            "",
            claims);

        var requiresProfileCompletion = string.IsNullOrWhiteSpace(user.AgencyName)
    || string.IsNullOrWhiteSpace(user.ContactPerson)
    || string.IsNullOrWhiteSpace(user.AgencyAddress)
    || string.IsNullOrWhiteSpace(user.Ico)
    || user.ServiceGoals == null || !user.ServiceGoals.Any();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RequiresProfileCompletion = requiresProfileCompletion
        };
    }
}
