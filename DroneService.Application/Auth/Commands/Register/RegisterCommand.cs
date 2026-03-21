using DroneService.Application.Contracts.Result;
using MediatR;

namespace DroneService.Application.Auth.Commands.Register;

// Command → reprezentuje akci "zaregistruj nového uživatele"
// IRequest<Result<string>> znamená:
// → handler vrátí výsledek (success/fail) + string (u tebe confirmation token)
public class RegisterCommand : IRequest<Result<string>>
{
    // DisplayName → ve tvém případě slouží jako email (!)
    // to je trochu matoucí název
    public string DisplayName { get; set; } = null!;

    // Heslo uživatele
    // validuje se později v handleru (PasswordValidator)
    public string Password { get; set; } = null!;
}