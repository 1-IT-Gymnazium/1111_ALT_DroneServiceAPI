using DroneService.Data.Entities.Identity;

namespace DroneService.Application.Contracts.Services;

// Interface pro import polí z externího systému (např. LPIS / ArcGIS)
// Slouží k načtení dat o polích a jejich uložení do aplikace
public interface IFieldImportService
{
    // Importuje pole pro konkrétního uživatele na základě ArcGIS (LPIS) ID
    Task ImportFieldsFromLpisAsync(
        AppUser user,   // uživatel, kterému budou pole přiřazena
        string arcGisId // identifikátor z externího systému (LPIS / ArcGIS)
    );
}