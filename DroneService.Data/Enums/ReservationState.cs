namespace DroneService.Data.Enums;

public enum ReservationState
{
    Created = 0,
    WaitingForConfirmation = 1,
    Confirmed = 2,
    WaitingForPayment = 3,
    Paid = 4,
    Unscheduled = 5,
    Scheduled = 6,
    Completed = 7,
    Cancelled = 8,
}
