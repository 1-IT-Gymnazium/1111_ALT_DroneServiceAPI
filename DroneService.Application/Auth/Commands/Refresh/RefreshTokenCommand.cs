using DroneService.Application.Contracts.Auth;
using MediatR;

namespace DroneService.Application.Auth.Commands.Refresh;

// Command → reprezentuje akci "obnov access token pomocí refresh tokenu"
// IRequest<LoginResponse> znamená:
// → handler vrátí nové tokeny (access + refresh)
public class RefreshTokenCommand : IRequest<LoginResponse>
{
    // Refresh token (přijde z cookie nebo requestu)
    // používá se k ověření identity bez nutnosti loginu
    public string RefreshToken { get; set; } = null!;

    // Informace o requestu (IP, zařízení, browser…)
    // používá se pro bezpečnost / audit
    public string RequestInfo { get; set; } = null!;
}