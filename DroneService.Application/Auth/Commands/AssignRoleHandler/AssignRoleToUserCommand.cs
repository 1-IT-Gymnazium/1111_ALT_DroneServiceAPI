using DroneService.Application.Contracts.Result;
using MediatR;

namespace DroneService.Application.Auth.Commands.AssignRoleHandler;

// Command → reprezentuje akci "přiřaď roli uživateli"
// IRequest<Result> znamená:
// → handler vrátí objekt Result (úspěch / chyba + zpráva)
public class AssignRoleToUserCommand : IRequest<Result>
{
    // ID uživatele, kterému chceme roli přiřadit
    public Guid UserId { get; set; }

    // Název role (např. "Admin", "User")
    public string RoleName { get; set; } = null!;
}