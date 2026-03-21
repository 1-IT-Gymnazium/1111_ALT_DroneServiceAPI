using DroneService.Application.Contracts.Reservations;
using MediatR;

namespace DroneService.Application.Reservation.Queries.GetReservationHistory;

// Query objekt pro získání historických rezervací
// (tj. rezervací, které už proběhly – řeší handler)
public class GetReservationHistoryQuery : IRequest<List<ReservationDto>>
{
    // Volitelný filtr podle uživatele
    // Pokud je null → vrátí historii všech uživatelů (typicky admin)
    // Pokud má hodnotu → vrátí historii konkrétního uživatele
    public Guid? AuthorId { get; set; }
}