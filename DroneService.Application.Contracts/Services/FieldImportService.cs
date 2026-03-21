using DroneService.Application.Contracts.Fields;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System.Text.Json;

namespace DroneService.Application.Contracts.Services;

// Implementace služby pro import polí z ArcGIS (LPIS)
public class FieldImportService : IFieldImportService
{
    private readonly AppDbContext _dbContext;   // databázový kontext
    private readonly HttpClient _httpClient;   // HTTP klient pro volání ArcGIS API
    private readonly IClock _clock;      // práce s časem (NodaTime)

    public FieldImportService(AppDbContext dbContext, HttpClient httpClient, IClock clock)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _clock = clock;
    }

    public async Task ImportFieldsFromLpisAsync(AppUser user, string arcGisId)
    {
        // načtení polí z ArcGIS podle LPIS ID
        var fields = await FetchFieldsFromArcGis(arcGisId);

        // aktuální čas
        var now = _clock.GetCurrentInstant();

        // vytvoření entit a jejich přiřazení uživateli
        foreach (var f in fields)
        {
            var field = new Field
            {
                Id = Guid.NewGuid(),
                Area = f.Area,
                AtticBlock = f.AtticBlock,
                BlockType = f.BlockType,
                Municipality = f.Municipality,
                LpisId = f.LpisId,
                FID = f.FID,
                dDpb = f.dDpb,
            };

            // nastavení auditních hodnot (CreatedAt apod.)
            field.SetCreateBySystem(now);

            // přidání pole k uživateli (navázání relace)
            user.Fields.Add(field);
        }

        // uložení změn do databáze
        await _dbContext.SaveChangesAsync();
    }

    // privátní metoda pro získání dat z ArcGIS REST API
    private async Task<List<FieldDto>> FetchFieldsFromArcGis(string arcGisId)
    {
        // sestavení URL pro dotaz na ArcGIS
        var url =
            $"https://services4.arcgis.com/8rJaSIyz8pUtbEBm/arcgis/rest/services/Map/FeatureServer/0/query" +
            $"?where=LPIS_ID='{arcGisId}'&outFields=*&f=json";

        // stažení JSON odpovědi
        var response = await _httpClient.GetStringAsync(url);

        // nastavení deserializace (ignoruje velikost písmen)
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // deserializace JSON na objekt
        var result = JsonSerializer.Deserialize<ArcGisResponse>(response, options);

        // pokud nic nepřišlo → vrátí prázdný seznam
        if (result == null || result.Features == null)
            return new List<FieldDto>();

        // mapování ArcGIS dat na interní DTO
        return result.Features.Select(f => new FieldDto
        {
            Area = f.Attributes.VYMERA ?? 0,
            AtticBlock = f.Attributes.ZKOD_DPB,
            BlockType = f.Attributes.KULTURANAZ,
            Municipality = f.Attributes.PRISL_OPZL,
            LpisId = Convert.ToInt32(f.Attributes.ID_UZ ?? "0"),
            FID = Convert.ToInt32(f.Attributes.FID ?? "0"),
            dDpb = Convert.ToInt32(f.Attributes.ID_DPB ?? "0"),
        }).ToList();
    }
}