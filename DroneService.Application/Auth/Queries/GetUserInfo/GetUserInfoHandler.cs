using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

// Handler → vrací základní informace o aktuálně přihlášeném uživateli
public class GetUserInfoHandler
    : IRequestHandler<GetUserInfoQuery, Result<LoggedUserModel>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserInfoHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<LoggedUserModel>> Handle(
        GetUserInfoQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. ZÍSKÁNÍ USER ID Z CLAIMS
        // =========================================
        // ClaimTypes.NameIdentifier obsahuje ID uživatele z JWT tokenu
        var userId = request.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // =========================================
        // 2. NAČTENÍ UŽIVATELE Z DB
        // =========================================
        var user = await _userManager.FindByIdAsync(userId);

        // Pokud uživatel neexistuje → vrátíme chybu
        if (user == null)
            return Result<LoggedUserModel>.Fail("USER_NOT_FOUND");

        // =========================================
        // 3. VYTVOŘENÍ DTO
        // =========================================
        var model = new LoggedUserModel
        {
            id = user.Id,

            // víme, že user existuje → je přihlášený
            isAuthenticated = true,

            // zatím natvrdo false (role se neřeší)
            isAdmin = false,
        };

        // =========================================
        // 4. RETURN
        // =========================================
        return Result<LoggedUserModel>.Ok(model);
    }
}