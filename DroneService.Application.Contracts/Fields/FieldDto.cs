using System.Globalization;
using System.Text.Json;

namespace DroneService.Application.Contracts.Fields; 
public class FieldDto
{
    public double Area { get; set; }           // VYMERA
    public string AtticBlock { get; set; } = null!;    // ZKOD_DPB
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
                  $"?where=ID_UZ='{lpisId}'&outFields=*&f=json";

        var response = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);

        if (!doc.RootElement.TryGetProperty("features", out var features))
            throw new InvalidOperationException($"Žádné pole pro LPIS_ID {lpisId} nenalezeno.");

        var results = new List<FieldDto>();

        foreach (var feature in features.EnumerateArray())
        {
            var attrs = feature.GetProperty("attributes");

            // VYMERA
            var vymElem = attrs.GetProperty("VYMERA");
            double area = vymElem.ValueKind switch
            {
                JsonValueKind.Number => vymElem.GetDouble(),
                JsonValueKind.String when
                    double.TryParse(vymElem.GetString(), NumberStyles.Any,
                                    CultureInfo.InvariantCulture, out var d)
                    => d,
                _ => 0
            };

            // Textové vlastnosti
            string atticBlock = attrs.GetProperty("ZKOD_DPB").GetString() ?? string.Empty;
            string blockType = attrs.GetProperty("KULTURANAZ").GetString() ?? string.Empty;
            string municipality = attrs.GetProperty("PRISL_OPZL").GetString() ?? string.Empty;

            results.Add(new FieldDto
            {
                Area = area,
                AtticBlock = atticBlock,
                BlockType = blockType,
                Municipality = municipality
            });
        }
        return results;
    }
}

