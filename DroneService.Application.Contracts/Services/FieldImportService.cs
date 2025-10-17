using DroneService.Application.Contracts.Fields;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System.Text.Json;

namespace DroneService.Application.Contracts.Services;

public class FieldImportService : IFieldImportService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _http;
    private readonly IClock _clock;

    public FieldImportService(AppDbContext db, HttpClient http, IClock clock)
    {
        _db = db;
        _http = http;
        _clock = clock;
    }

    public async Task ImportFieldsFromLpisAsync(AppUser user, string arcGisId)
    {

        var fields = await FetchFieldsFromArcGis(arcGisId);
        var now = _clock.GetCurrentInstant();

        foreach (var f in fields)
        {
            var field = new Field
            {
                Id = Guid.NewGuid(),
                Area = f.Area,
                AtticBlock = f.AtticBlock,
                BlockType = f.BlockType,
                Municipality = f.Municipality,
            };

            field.SetCreateBySystem(now);

            user.Fields.Add(field);
        }

        await _db.SaveChangesAsync();
    }

    private async Task<List<FieldDto>> FetchFieldsFromArcGis(string arcGisId)
    {
        var url = $"https://services4.arcgis.com/8rJaSIyz8pUtbEBm/arcgis/rest/services/Map/FeatureServer/0/query" + $"?where=LPIS_ID='{arcGisId}'&outFields=*&f=json";

        var response = await _http.GetStringAsync(url);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ArcGisResponse>(response, options);

        if (result == null || result.Features == null)
            return new List<FieldDto>();

        return result.Features.Select(f => new FieldDto
        {
            Area = f.Attributes.VYMERA ?? 0,
            AtticBlock = f.Attributes.ZKOD_DPB,
            BlockType = f.Attributes.KULTURANAZ,
            Municipality = f.Attributes.PRISL_OPZL,

        }).ToList();
    }

}
