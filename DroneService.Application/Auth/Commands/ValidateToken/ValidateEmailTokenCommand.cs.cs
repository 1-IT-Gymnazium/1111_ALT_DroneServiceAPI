
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DroneService.Application.Auth.Commands.ValidateToken;

public class ValidateEmailTokenCommand : IRequest<IResult>
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
