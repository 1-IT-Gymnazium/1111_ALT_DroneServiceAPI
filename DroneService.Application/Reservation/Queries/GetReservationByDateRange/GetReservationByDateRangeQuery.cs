using DroneService.Application.Contracts.Reservations;
using MediatR;
using NodaTime;

namespace DroneService.Application.Reservation.Queries.GetReservationByDateRange;

// Tento Query objekt slouží pro získání rezervací v určitém časovém rozsahu.
// Na rozdíl od filtrovací query je jednodušší – bere jen počáteční datum a počet dní.
public record GetReservationsByDateRangeQuery(

    // Počáteční datum (od kdy chceme rezervace)
    // Používá se jako začátek časového intervalu
    Instant From,

    // Počet dní od počátečního data
    // Např. Days = 7 → vrátí rezervace v intervalu From → From + 7 dní
    int Days

) : IRequest<List<DetailReservationModel>>;