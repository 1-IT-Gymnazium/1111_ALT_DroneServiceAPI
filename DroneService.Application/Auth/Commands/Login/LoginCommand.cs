using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using MediatR;

namespace DroneService.Application.Auth.Commands.Login;

// Command → reprezentuje akci "přihlásit uživatele"
// IRequest<Result<LoginResponse>> znamená:
// → handler vrátí výsledek (success/fail) + data (tokeny, info)
public class LoginCommand : IRequest<Result<LoginResponse>>
{
    // Email uživatele (používá se pro vyhledání v DB)
    public string Email { get; set; } = null!;

    // Heslo (ověřuje se přes SignInManager / Identity)
    public string Password { get; set; } = null!;
}