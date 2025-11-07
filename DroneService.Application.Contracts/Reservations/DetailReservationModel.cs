using DroneService.Application.Contracts.Utils;
using DroneService.Data.Entities;

namespace DroneService.Application.Contracts.Reservations;

public class DetailReservationModel
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsSubscription { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string ModifiedAt { get; set; } = null!;
}

public static class DetailReservationModelExtensions
{
    public static DetailReservationModel ToDetail(this IApplicationMapper mapper, Reservation source)
        => new()
        {
            Id = source.Id,
            ScheduledAt = source.ScheduledAt,
            ServiceType = source.ServiceType,
            Price = source.Price,
            CreatedAt = source.CreatedAt.ToString(),
            ModifiedAt = source.ModifiedAt.ToString(),
        };
}
