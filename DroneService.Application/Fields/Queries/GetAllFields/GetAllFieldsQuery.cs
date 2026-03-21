using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Queries.GetAllFields;

// Query → slouží k získání seznamu polí (s možností filtrování a řazení)
// typicky používané pro admin přehled nebo list view
public class GetAllFieldsQuery : IRequest<List<DetailFieldModel>>
{
    // =========================================
    // FILTRY
    // =========================================

    // filtr podle názvu pole
    public string? Name { get; set; }

    // filtr podle obce
    public string? Municipality { get; set; }

    // filtr podle plodiny
    public string? CurrentCrops { get; set; }

    // filtr podle typu bloku
    public string? BlockType { get; set; }

    // minimální rozloha
    public double? MinArea { get; set; }

    // maximální rozloha
    public double? MaxArea { get; set; }

    // =========================================
    // ŘAZENÍ
    // =========================================

    // podle čeho řadit (např. "name", "area", "createdAt")
    public string? SortBy { get; set; }

    // směr řazení ("asc" / "desc")
    public string? SortDirection { get; set; }
}