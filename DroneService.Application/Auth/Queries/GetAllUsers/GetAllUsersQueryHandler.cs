using DroneService.Application.Contracts.Auth;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<AppUserDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetAllUsersQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AppUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = _userManager.Users.ToList();

        var result = await Task.WhenAll(users.Select(async user =>
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return new AppUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Role = role ?? "user"
            };
        }));

        return result.ToList();

    }
}

