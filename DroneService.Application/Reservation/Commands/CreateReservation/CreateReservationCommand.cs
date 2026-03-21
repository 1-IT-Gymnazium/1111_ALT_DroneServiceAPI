using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Result;
using MediatR;
using NodaTime;

namespace DroneService.Application.Reservation.Commands.CreateReservation;

// Command → reprezentuje požadavek na vytvoření nové rezervace
// obsahuje všechna data potřebná pro vytvoření (pole + čas)
public class CreateReservationCommand : IRequest<Result<DetailReservationModel>>
{
    // seznam ID polí, na která se rezervace vztahuje
    // může jich být více (např. jedna objednávka pro více polí)
    public List<Guid> FieldIds { get; set; } = new();

    // čas rezervace (Instant = přesný UTC čas)
    public Instant ScheduledAt { get; set; }
}