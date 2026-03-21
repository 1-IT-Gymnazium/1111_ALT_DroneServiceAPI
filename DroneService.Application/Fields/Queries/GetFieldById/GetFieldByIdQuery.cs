using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Queries.GetUsersFields;

// Query → slouží k získání jednoho konkrétního pole podle ID
// Používá se v rámci MediatR (CQS pattern)
public class GetFieldByIdQuery : IRequest<DetailFieldModel>
{
    // ID pole, které chceme načíst z databáze
    public Guid Id { get; set; }

    // Konstruktor → nastaví ID při vytvoření query
    public GetFieldByIdQuery(Guid id)
    {
        Id = id;
    }
}