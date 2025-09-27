using DroneService.Application.Contracts.Interfaces;
using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using DroneService.Utilities.Options;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NodaTime;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IClock _clock;
    private readonly IEmailSenderService _emailSenderService;
    private readonly EnvironmentOptions _env;
    private readonly JwtSetting _jwt;

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IClock clock,
        IEmailSenderService emailSenderService,
        IOptions<EnvironmentOptions> env,
        IOptions<JwtSetting> jwt)
    {
        _userManager = userManager;
        _clock = clock;
        _emailSenderService = emailSenderService;
        _env = env.Value;
        _jwt = jwt.Value;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.GetCurrentInstant();

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName, //email
            UserName = request.DisplayName, //email
        }.SetCreateBySystem(now);

        var passwordValidation = await new PasswordValidator<AppUser>().ValidateAsync(_userManager, user, request.Password);
        if (!passwordValidation.Succeeded)
        {
            throw new ValidationException(string.Join(", ", passwordValidation.Errors.Select(e => e.Description)));
        }

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            throw new Exception("USER_CREATION_FAILED");
        }

        var existingClaims = await _userManager.GetClaimsAsync(user);
        if (!existingClaims.Any(c => c.Type == "role" && c.Value == "User"))
        {
            await _userManager.AddClaimAsync(user, new Claim("role", "User"));
        }

        await _userManager.AddPasswordAsync(user, request.Password);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var url = $"{_env.FrontendHostUrl.TrimEnd('/')}/{_env.FrontendConfirmUrl.TrimStart('/')}?token={Uri.EscapeDataString(token)}&email={user.DisplayName}";

        await _emailSenderService.AddEmail(
            "Registrace",
            $"<a href=\"{url}\">Potvrď svůj účet</a>",
            request.DisplayName,
            request.DisplayName
        );

        return token;
    }
}
