using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using MediatR;
using System.Security.Claims;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

// Query → požadavek na získání informací o aktuálně přihlášeném uživateli
public class GetUserInfoQuery : IRequest<Result<LoggedUserModel>>
{
    // Celý uživatel (ClaimsPrincipal) z ASP.NET kontextu
    // Obsahuje:
    // - ID (NameIdentifier)
    // - role
    // - další claims z JWT
    public ClaimsPrincipal User { get; set; } = null!;
}