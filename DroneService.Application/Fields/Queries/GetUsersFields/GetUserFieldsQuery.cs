using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Queries.GetUsersFields;

// Query → slouží k získání všech polí konkrétního uživatele
// Používá se v rámci MediatR (CQS pattern)
public record GetUserFieldsQuery(Guid AuthorId): IRequest<List<DetailFieldModel>>;