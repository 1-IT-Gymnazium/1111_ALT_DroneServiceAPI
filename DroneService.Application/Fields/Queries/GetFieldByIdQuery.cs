using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Queries;

public class GetFieldByIdQuery : IRequest<DetailFieldModel>
{
    public Guid Id { get; set; }

    public GetFieldByIdQuery(Guid id)
    {
        Id = id;
    }
}
