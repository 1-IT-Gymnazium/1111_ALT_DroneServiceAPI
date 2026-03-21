using MediatR;
using Microsoft.AspNetCore.Http;

namespace DroneService.Application.Auth.Commands.ValidateToken;

// Command → potvrzení emailu pomocí tokenu
// IRequest<IResult>:
// → handler vrátí HTTP výsledek (např. OK / BadRequest)
public class ValidateEmailTokenCommand : IRequest<IResult>
{
    // Email uživatele, který potvrzuje účet
    public string Email { get; set; } = null!;

    // Token z emailu (vygenerovaný při registraci)
    public string Token { get; set; } = null!;
}