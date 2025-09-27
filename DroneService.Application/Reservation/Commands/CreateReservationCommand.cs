using MediatR;

namespace DroneService.Application.Reservations.Commands;

public class CreateReservationCommand : IRequest<Guid>
{
    public Guid UserId { get; set; } 
    public DateTime Date { get; set; }
    public string Location { get; set; } = null!;
    public string ServiceType { get; set; } = null!;
    public string Note { get; set; } = null!;
}