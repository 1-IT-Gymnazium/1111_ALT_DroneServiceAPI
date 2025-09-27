using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Commands;

public class UpdateFieldCommand : IRequest<DetailFieldModel>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public string CurrentCrops { get; set; }
    public int AtticBlock { get; set; }
    public string BlockType { get; set; } = null!;
    public string Municipality { get; set; } = null!;

    public UpdateFieldCommand(Guid id, CreateFieldModel model)
    {
        Id = id;
        Name = model.Name;
        Area = model.Area;
        CurrentCrops = model.CurrentCrops;
        AtticBlock = model.AtticBlock;
        BlockType = model.BlockType;
        Municipality = model.Municipality;
    }
}
