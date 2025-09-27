using DroneService.Application.Auth.Commands.Register;
using DroneService.Application.Contracts.Auth;
using MediatR;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

public class GetUserByAgencyName : IRequest<UserInfo>
{
    public string UserId { get; set; }

    public GetUserByAgencyName(string userId)
    {
        UserId = userId;
    }
}
