using MediatR;

namespace DroneService.Application.Fields.Commands;
public class DeleteFieldCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteFieldCommand(Guid id)
    {
        Id = id;
    }
}
