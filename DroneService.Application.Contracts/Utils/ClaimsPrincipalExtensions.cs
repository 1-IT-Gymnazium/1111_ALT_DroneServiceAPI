using System.Security.Claims;

namespace DroneService.Application.Contracts.Utils;

// Rozšiřující metody pro ClaimsPrincipal (aktuálně přihlášený uživatel)
// Umožňují jednoduše získat data z JWT tokenu (claims)
public static class ClaimsPrincipalExtensions
{
    public static string GetName(this ClaimsPrincipal user)
    {
        // kontrola, zda je uživatel přihlášen
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("user not logged in");
        }

        // získání jména z claimu (ClaimTypes.Name)
        var name = user.Claims
            .First(x => x.Type == ClaimTypes.Name)
            .Value;

        return name;
    }

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // kontrola, zda je uživatel přihlášen
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("user not logged in");
        }

        // získání ID uživatele z claimu (ClaimTypes.NameIdentifier)
        var idString = user.Claims
            .First(x => x.Type == ClaimTypes.NameIdentifier)
            .Value;

        // převod stringu na Guid
        return Guid.Parse(idString);
    }
}