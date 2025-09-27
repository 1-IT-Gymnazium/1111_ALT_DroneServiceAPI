using DroneService.Application.Contracts.Auth;
using MediatR;

namespace DroneService.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

