using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, LoggedUserModel>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserInfoQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<LoggedUserModel> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var userId = request.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) throw new Exception("USER_NOT_FOUND");

        return new LoggedUserModel
        {
            id = user.Id,
            isAuthenticated = true,
            isAdmin = false,
        };
    }
}
