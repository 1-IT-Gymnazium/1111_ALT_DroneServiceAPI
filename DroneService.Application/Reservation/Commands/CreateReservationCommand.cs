using DroneService.Application.Contracts.Reservations;
using MediatR;

namespace DroneService.Application.Reservations.Commands;

public class CreateReservationCommand : IRequest<DetailReservationModel>
{
    public List<Guid> FieldIds { get; set; } = new();
    public DateTime ScheduledAt { get; set; }
}