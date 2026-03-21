using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data.Entities;
using NodaTime;

namespace DroneService.Application.Contracts.Reservations;

public class DetailReservationModel
{
    public Guid Id { get; set; }
    public Instant ScheduledAt { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsSubscription { get; set; }
    public string State { get; set; } = null!;
    public string AgencyName { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = null!;
    public string ModifiedAt { get; set; } = null!;
    public List<DetailFieldModel> Fields { get; set; } = new();
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
            State = source.State.ToString(),
            AgencyName = source.Author?.AgencyName ?? "—",
            CreatedAt = source.CreatedAt.ToString(),
            ModifiedAt = source.ModifiedAt.ToString(),
            Fields = source.Fields
                .Select(f => mapper.ToDetailField(f))
                .ToList()
        };
}
