using MediatR;

namespace DroneService.Application.Reservation.Commands.DeleteReservation;

// Command → reprezentuje požadavek na smazání rezervace
// neobsahuje žádnou logiku, pouze nese data do handleru
public class DeleteReservationCommand : IRequest<bool>
{
    // ID rezervace, kterou chceme smazat
    public Guid Id { get; set; }

    // konstruktor → nastaví ID rezervace
    public DeleteReservationCommand(Guid id)
    {
        Id = id;
    }
}