using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.ServiceType;
using DroneService.Data.Entities.Identity;
using MediatR;
namespace DroneService.Application.ServiceTypes.Command;

public class CreateServiceCommand : IRequest<DetailServiceModel>
{
    public string Name { get; set; }
    public bool IsSubscription { get; set; }
    public Guid AuthorId { get; set; }
    public CreateServiceCommand(CreateServiceModel model, Guid authorId)
    {
        Name = model.Name;
        IsSubscription = model.IsSubscription;
        AuthorId = authorId;
    }
}

