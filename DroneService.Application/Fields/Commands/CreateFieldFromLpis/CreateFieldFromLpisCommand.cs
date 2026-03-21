using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Commands.CreateFieldFromLpis;

// Command → vytvoření pole (field) na základě LPIS dat
// LPIS = externí systém (typicky data o zemědělských pozemcích)
public class CreateFieldFromLpisCommand : IRequest<DetailFieldModel>
{
    // ID z LPIS systému
    // podle něj se budou tahat data z externí služby (např. ArcGIS)
    public string LpisId { get; set; } = null!;

    // ID uživatele, který pole vytváří
    // nastavuje se typicky v controlleru z JWT tokenu
    public Guid AuthorId { get; set; }
}