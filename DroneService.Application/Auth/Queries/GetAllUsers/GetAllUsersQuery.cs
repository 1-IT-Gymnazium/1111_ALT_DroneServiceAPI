using DroneService.Application.Contracts.Auth;
using MediatR;
using NodaTime;

namespace DroneService.Application.Auth.Queries.GetAllUsers;

// Query objekt – reprezentuje požadavek na získání seznamu uživatelů
// Obsahuje všechny možné filtry, které může admin použít
public class GetAllUsersQuery : IRequest<List<AdminUserListDto>>
{
    // Hledání podle jména nebo emailu
    // Používá se pro obecné vyhledávání
    public string? DisplayName { get; set; }

    // Filtr podle role (např. "Admin", "User")
    // Pozor: v handleru se řeší přes claims
    public string? Role { get; set; }

    // Název agentury
    public string? AgencyName { get; set; }

    // Kontaktní osoba
    public string? ContactPerson { get; set; }

    // Adresa agentury
    public string? AgencyAddress { get; set; }

    // IČO firmy
    public string? Ico { get; set; }

    // Seznam cílů služeb
    // Např. ["Postřik", "Monitoring"]
    // Používá se pro pokročilé filtrování
    public List<string>? ServiceGoals { get; set; }

    // Řazení výsledků
    // očekávané hodnoty např.:
    // "oldest" → od nejstarších
    // jinak default → od nejnovějších
    public string? Sort { get; set; }

    // Filtr podle data vytvoření
    // vrátí jen uživatele vytvořené od tohoto data
    public Instant? CreatedFrom { get; set; }
}