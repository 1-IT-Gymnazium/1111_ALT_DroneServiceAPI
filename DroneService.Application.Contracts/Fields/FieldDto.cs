using System.Text.Json;

namespace DroneService.Application.Contracts.Fields;

public class FieldDto
{
    public double Area { get; set; }           // VYMERA
    public int AtticBlock { get; set; }        // ZKOD_DPB
    public string BlockType { get; set; } = null!;   // KULTURANAZ
    public string Municipality { get; set; } = null!; // PRISL_OPZL
}

public class ArcGisService
{
    private readonly HttpClient _httpClient;

    public ArcGisService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FieldDto>> GetFieldsByLpisIdAsync(string lpisId)
    {
        var url = $"https://services4.arcgis.com/8rJaSIyz8pUtbEBm/arcgis/rest/services/Map/FeatureServer/0/query" +
                  $"?where=LPIS_ID='{lpisId}'&outFields=*&f=json";

        var response = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var features = doc.RootElement.GetProperty("features");

        var results = new List<FieldDto>();
        foreach (var feature in features.EnumerateArray())
        {
            var attributes = feature.GetProperty("attributes");

            results.Add(new FieldDto
            {
                Area = attributes.GetProperty("VYMERA").GetDouble(),
                AtticBlock = attributes.GetProperty("ZKOD_DPB").GetInt32(),
                BlockType = attributes.GetProperty("KULTURANAZ").GetString() ?? string.Empty,
                Municipality = attributes.GetProperty("PRISL_OPZL").GetString() ?? string.Empty
            });
        }

        return results;
    }
}
