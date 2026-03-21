namespace DroneService.Application.Contracts.Interfaces;

// Interface sloužící pro získání informací o aktuálně přihlášeném uživateli
// Typicky se implementuje pomocí HttpContext (např. z JWT tokenu)
public interface IUserContext
{
    // Vrací ID aktuálně přihlášeného uživatele
    // Používá se např. v Handlerech místo přímé práce s HttpContext
    Guid GetUserId();
}