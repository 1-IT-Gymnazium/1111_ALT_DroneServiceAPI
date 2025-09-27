using DroneService.Application.Contracts.Auth;
using MediatR;
using System.Security.Claims;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

public class GetUserInfoQuery : IRequest<LoggedUserModel>
{
    public ClaimsPrincipal User { get; set; } = null!;
}
