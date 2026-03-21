using MediatR;

namespace DroneService.Application.Auth.Commands.Logout;

// Command → reprezentuje akci "odhlásit uživatele"
// IRequest<Unit> znamená:
// → nevrací žádná data (Unit = něco jako void v MediatR)
public class LogoutCommand : IRequest<Unit>
{
    // Refresh token (přijde z cookie)
    // může být null → proto string?
    public string? RefreshToken { get; set; }
}