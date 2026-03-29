using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Result;
using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using DroneService.Utilities.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NodaTime;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.Register;

// Handler → registrace nového uživatele
// Vrací Result<string> → string = email confirmation token
public class RegisterHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IClock _clock;
    private readonly IEmailSenderService _emailSenderService;
    private readonly EnvironmentOptions _envOptions;

    public RegisterHandler(
        UserManager<AppUser> userManager,
        IClock clock,
        IEmailSenderService emailSenderService,
        IOptions<EnvironmentOptions> envOptions)
    {
        _userManager = userManager;
        _clock = clock;
        _emailSenderService = emailSenderService;
        _envOptions = envOptions.Value;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // =========================================
        // 1. AKTUÁLNÍ ČAS (NodaTime)
        // =========================================
        var now = _clock.GetCurrentInstant();

        // =========================================
        // 2. VYTVOŘENÍ USERA (bez hesla zatím)
        // =========================================
        var user = new AppUser
        {
            Id = Guid.NewGuid(),

            // Používáš email jako username
            DisplayName = request.DisplayName,
            UserName = request.DisplayName,
        }
        // vlastní metoda → nastaví CreatedAt apod.
        .SetCreateBySystem(now);

        // =========================================
        // 3. VALIDACE HESLA
        // =========================================
        var passwordValidation = await new PasswordValidator<AppUser>()
            .ValidateAsync(_userManager, user, request.Password);

        if (!passwordValidation.Succeeded)
        {
            var errors = passwordValidation.Errors
                .Select(e => e.Description)
                .ToList();

            var errorMessage = string.Join(", ", errors);

            return Result<string>.Fail(errorMessage);
        }

        // =========================================
        // 4. VYTVOŘENÍ USERA V DB (bez hesla!)
        // =========================================
        var createResult = await _userManager.CreateAsync(user);

        if (!createResult.Succeeded)
        {
            // ⚠️ tahle hláška není přesná
            return Result<string>.Fail("Uživatel neexistuje");
        }

        // =========================================
        // 5. PŘIŘAZENÍ ROLE (default "User")
        // =========================================
        var existingClaims = await _userManager.GetClaimsAsync(user);

        if (!existingClaims.Any(c => c.Type == "role" && c.Value == "User"))
        {
            await _userManager.AddClaimAsync(user, new Claim("role", "User"));
        }

        // =========================================
        // 6. NASTAVENÍ HESLA
        // =========================================
        await _userManager.AddPasswordAsync(user, request.Password);

        // =========================================
        // 7. GENEROVÁNÍ EMAIL CONFIRMATION TOKENU
        // =========================================
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // =========================================
        // 8. VYTVOŘENÍ URL PRO POTVRZENÍ
        // =========================================
        var url =
            $"{_envOptions.FrontendHostUrl.TrimEnd('/')}/" +
            $"{_envOptions.FrontendConfirmUrl.TrimStart('/')}" +
            $"?token={Uri.EscapeDataString(token)}&email={user.DisplayName}";

        // =========================================
        // 9. ODESLÁNÍ EMAILU (async přes background service)
        // =========================================
        await _emailSenderService.AddEmail(
            "Registrace",
            $"<a href=\"{url}\">Potvrď svůj účet</a>",
            request.DisplayName,
            request.DisplayName
        );

        // Vracíme token (např. pro debug nebo frontend)
        return Result<string>.Ok(token);
    }
}