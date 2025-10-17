using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Commands;

public class UpdateFieldCommand : IRequest<DetailFieldModel>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CurrentCrops { get; set; }

    public UpdateFieldCommand(Guid id, CreateFieldModel model)
    {
        Id = id;
        Name = model.Name;
        CurrentCrops = model.CurrentCrops;
    }
}
