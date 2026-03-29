using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Auth.Commands.ValidateToken;

// Handler → ověří email pomocí tokenu
public class ValidateEmailTokenHandler
    : IRequestHandler<ValidateEmailTokenCommand, IResult>
{
    private readonly UserManager<AppUser> _userManager;

    public ValidateEmailTokenHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IResult> Handle(ValidateEmailTokenCommand request, CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NORMALIZACE EMAILU
        // =========================================
        // Identity ukládá emaily/username ve velkých písmenech
        var normalizedEmail = request.Email.ToUpperInvariant();

        // =========================================
        // 2. NAJÍT USERA, KTERÝ JEŠTĚ NENÍ POTVRZENÝ
        // =========================================
        var user = await _userManager.Users
            .SingleOrDefaultAsync(
                x => !x.EmailConfirmed && x.NormalizedUserName == normalizedEmail,
                cancellationToken
            );

        // Pokud user neexistuje nebo už je potvrzený → fail
        if (user == null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { nameof(request.Token), new[] { "INVALID_TOKEN" } }
            });
        }

        // =========================================
        // 3. OVĚŘENÍ TOKENU
        // =========================================
        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        // Token nesedí / expiroval → fail
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { nameof(request.Token), new[] { "INVALID_TOKEN" } }
            });
        }

        // =========================================
        // 4. HOTOVO ✅
        // =========================================
        // EmailConfirmed = true (nastaví Identity interně)
        return Results.NoContent();
    }
}