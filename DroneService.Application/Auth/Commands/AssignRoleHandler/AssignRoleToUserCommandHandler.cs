using DroneService.Application.Contracts.Result;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.AssignRoleHandler;

// Handler pro command AssignRoleToUserCommand
// Vrací Result → vlastní wrapper pro úspěch / chybu
public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Result>
{
    // UserManager = hlavní nástroj ASP.NET Identity pro práci s uživateli
    private readonly UserManager<AppUser> _userManager;

    public AssignRoleToUserCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        // Najdeme uživatele podle ID
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        // Pokud neexistuje → vrátíme chybu
        if (user == null)
            return Result.Fail("USER_NOT_FOUND");

        // Načteme všechny claimy uživatele
        var existingClaims = await _userManager.GetClaimsAsync(user);

        // Projdeme všechny role claimy a odstraníme je
        // → tím zajistíme, že uživatel má vždy jen jednu roli
        foreach (var claim in existingClaims.Where(c => c.Type == ClaimTypes.Role))
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        // Přidáme novou roli jako claim
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, request.RoleName));

        // Vracíme úspěch
        return Result.Ok();
    }
}