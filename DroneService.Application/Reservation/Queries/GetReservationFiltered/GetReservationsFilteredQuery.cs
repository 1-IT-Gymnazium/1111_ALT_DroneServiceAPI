using DroneService.Application.Contracts.Reservations;
using DroneService.Data.Enums;
using MediatR;
using NodaTime;

namespace DroneService.Application.Reservation.Queries.GetReservationFiltered;

// Tento Query objekt slouží pro získání rezervací podle různých filtrů.
// Je to flexibilnější verze než "GetUserReservationQuery", protože umožňuje kombinovat více podmínek.
public record GetReservationsFilteredQuery(

    // Od kdy chceme rezervace (časový filtr - začátek)
    // Pokud je null → nefiltruje se podle "od"
    Instant? From,

    // Do kdy chceme rezervace (časový filtr - konec)
    // Pokud je null → nefiltruje se podle "do"
    Instant? To,

    // Stav rezervace (např. Created, Approved, Cancelled...)
    // Pokud je null → vrátí všechny stavy
    ReservationState? State,

    // Typ služby (např. "Basic", "Premium", "Scan"...)
    // Pokud je null → všechny typy
    string? ServiceType,

    // Filtrování podle konkrétního pole
    // Pokud je null → všechny pole
    Guid? FieldId,

    // Filtrování podle uživatele (admin vs user scénář)
    // Pokud je null → může vrátit rezervace všech uživatelů (typicky admin)
    Guid? UserId

) : IRequest<List<DetailReservationModel>>;