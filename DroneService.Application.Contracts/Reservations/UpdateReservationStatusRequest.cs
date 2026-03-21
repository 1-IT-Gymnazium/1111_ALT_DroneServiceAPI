using DroneService.Data.Enums;

namespace DroneService.Application.Contracts.Reservations;

public class UpdateReservationStatusRequest
{
    public ReservationState Status { get; set; }
}
