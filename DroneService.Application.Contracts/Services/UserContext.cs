using DroneService.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DroneService.Application.Contracts.Services;

// Implementace IUserContext
// Slouží k získání informací o aktuálně přihlášeném uživateli z HttpContextu (JWT tokenu)
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor; // umožňuje přístup k aktuálnímu HTTP requestu

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        // získání hodnoty claimu NameIdentifier (standardně obsahuje ID uživatele)
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // pokud ID není nalezeno → uživatel není autentizovaný nebo token neobsahuje ID
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found in token.");

        // převod stringu na Guid
        return Guid.Parse(userId);
    }
}