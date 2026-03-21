using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using NodaTime;

namespace DroneService.Application.Contracts.Utils;

// Interface pro mapování entit na DTO modely (Application layer)
// Slouží k oddělení databázových entit od dat vracených ven (např. API)
public interface IApplicationMapper
{
    Instant Now { get; } // aktuální čas (např. pro výpočty nebo mapování)

    // mapování uživatele (AppUser entity) na detailní DTO
    DetailUserModel ToDetailUser(AppUser user);
}