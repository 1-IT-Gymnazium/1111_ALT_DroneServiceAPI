using NodaTime;

namespace DroneService.Application.Contracts.Reservations;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Instant ScheduledAt { get; set; }
    public string ServiceType { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsSubscription { get; set; }
    public string State { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public List<string> FieldNames { get; set; } = new();
}
