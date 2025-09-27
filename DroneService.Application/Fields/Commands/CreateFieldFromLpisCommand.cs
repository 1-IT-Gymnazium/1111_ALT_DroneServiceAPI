
using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Commands;

public class CreateFieldFromLpisCommand : IRequest<DetailFieldModel>
{
    public string LpisId { get; set; } = null!;
    public Guid AuthorId { get; set; } 
}

