using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

public class GetUserByAgencyNameHandler : IRequestHandler<GetUserByAgencyName, UserInfo?>
{
    private readonly UserManager<AppUser> _userManager;

    public GetUserByAgencyNameHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserInfo?> Handle(GetUserByAgencyName request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
            return null; 

        var user = await _userManager.Users
            .Where(u => u.Id == userGuid)
            .Select(u => new UserInfo
            {
                UserName = u.UserName,
                Email = u.Email,
                AgencyName = u.AgencyName,
                AgencyAddress = u.AgencyAddress,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}