using DroneService.Application.Contracts.Reservations;
using DroneService.Data.Enums;
using MediatR;

namespace DroneService.Application.Reservation.Commands.UpdateReservationStatus;

// Command → reprezentuje požadavek na změnu stavu rezervace
// Používá se v MediatR jako vstup do handleru
public record UpdateReservationStatusCommand(
    Guid Id,                // ID rezervace, kterou chceme upravit
    ReservationState Status // nový stav rezervace (např. Approved, Rejected, ...)
) : IRequest<DetailReservationModel>;  // handler vrátí detail rezervace