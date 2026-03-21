using DroneService.Application.Contracts.Reservations;
using MediatR;

namespace DroneService.Application.Reservation.Queries.GetUserReservation;

// Query → reprezentuje požadavek na získání rezervací konkrétního uživatele
// obsahuje pouze vstupní data (žádná logika)
public record GetUserReservationQuery(Guid AuthorId)
    : IRequest<List<DetailReservationModel>>
{
}