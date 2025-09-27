using DroneService.Application.Contracts.Fields;
using DroneService.Data.Entities.Identity;
using MediatR;

namespace DroneService.Application.Fields.Commands;
public class CreateFieldCommand : IRequest<DetailFieldModel>
{
    public string Name { get; set; }
    public double Area { get; set; }
    public string CurrentCrops { get; set; }
    public int AtticBlock { get; set; }
    public string BlockType { get; set; } = null!;
    public string Municipality { get; set; } = null!;
    public AppUser Author { get; set; }
    public Guid AuthorId { get; set; }

    public CreateFieldCommand(CreateFieldModel model, Guid authorId)
    {
        Name = model.Name;
        Area = model.Area;
        CurrentCrops = model.CurrentCrops;
        AuthorId = authorId;
        AtticBlock = model.AtticBlock;
        BlockType = model.BlockType;
        Municipality = model.Municipality;
    }
}
