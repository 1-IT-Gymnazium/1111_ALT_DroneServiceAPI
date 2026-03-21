using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

// Handler → vrací základní informace o uživateli podle jeho ID
public class GetUserByAgencyNameHandler
    : IRequestHandler<GetUserByAgencyName, UserInfo?>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserByAgencyNameHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserInfo?> Handle(
        GetUserByAgencyName request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. VALIDACE ID
        // =========================================
        // UserId přichází jako string → pokus o převod na Guid
        if (!Guid.TryParse(request.UserId, out var userGuid))
            return null;

        // =========================================
        // 2. DOTAZ NA DB
        // =========================================
        var user = await _userManager.Users
            // filtr podle ID
            .Where(u => u.Id == userGuid)

            // projekce → nevracíme celý entity objekt,
            // ale jen data, která potřebujeme
            .Select(u => new UserInfo
            {
                UserName = u.UserName,
                Email = u.Email,
                AgencyName = u.AgencyName,
                AgencyAddress = u.AgencyAddress,
            })

            // vezmeme první nebo null
            .FirstOrDefaultAsync(cancellationToken);

        // =========================================
        // 3. RETURN
        // =========================================
        return user;
    }
}