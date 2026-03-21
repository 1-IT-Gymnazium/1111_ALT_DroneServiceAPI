using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using MediatR;

namespace DroneService.Application.Auth.Commands.Register;

// Command → reprezentuje vyplnění / aktualizaci profilu uživatele
// IRequest<Result<DetailUserModel>>:
// → handler vrátí aktualizovaného uživatele (DTO)
public class FormCommand : IRequest<Result<DetailUserModel>>
{
    // Název agentury (firma uživatele)
    public string AgencyName { get; set; } = null!;

    // Kontaktní osoba (jméno)
    public string ContactPerson { get; set; } = null!;

    // Adresa agentury
    public string AgencyAddress { get; set; } = null!;

    // IČO (identifikační číslo firmy)
    public string Ico { get; set; } = null!;

    // Seznam "cílů služeb" (např. typy práce, které uživatel dělá)
    // inicializováno na prázdný list → zabrání null reference
    public List<string> ServiceGoals { get; set; } = new();

    // Poznámka (volitelné info)
    public string Note { get; set; } = null!;
}