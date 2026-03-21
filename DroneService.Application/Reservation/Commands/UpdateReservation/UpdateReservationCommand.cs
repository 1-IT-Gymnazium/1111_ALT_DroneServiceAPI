using DroneService.Application.Contracts.Reservations;
using MediatR;
using NodaTime;
namespace DroneService.Application.Reservation.Commands.UpdateReservation;

// Command → reprezentuje požadavek na úpravu rezervace
// konkrétně: změna polí (FieldIds) a termínu (ScheduledAt)
public class UpdateReservationCommand : IRequest<DetailReservationModel>
{
    // ID rezervace, kterou chceme upravit
    public Guid Id { get; set; }

    // seznam ID polí, která jsou k rezervaci přiřazená
    // (např. uživatel si vybere více polí pro jednu rezervaci)
    public List<Guid> FieldIds { get; set; } = null!;

    // datum a čas rezervace (NodaTime → přesnější práce s časem než DateTime)
    public Instant ScheduledAt { get; set; }

    // konstruktor → jednoduše naplní command daty
    public UpdateReservationCommand(Guid id, List<Guid> fieldIds, Instant scheduledAt)
    {
        Id = id;
        FieldIds = fieldIds;
        ScheduledAt = scheduledAt;
    }
}