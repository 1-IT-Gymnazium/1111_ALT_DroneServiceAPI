using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Commands.AssignRoleHandler;

public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Unit>
{
    private readonly UserManager<AppUser> _userManager;

    public AssignRoleToUserCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new Exception("USER_NOT_FOUND");

        // Smazat předchozí role (volitelné)
        var existingClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in existingClaims.Where(c => c.Type == ClaimTypes.Role))
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }

        // Přidání nové role
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, request.RoleName));
        return Unit.Value;
    }
}

