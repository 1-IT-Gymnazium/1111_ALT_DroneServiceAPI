using DroneService.Application.Contracts.Utils;
using DroneService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Reservations;

public class DetailReservationModel
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Location { get; set; } = null!;
    public string ServiceType { get; set; } = null!;
    public string Note { get; set; } = null!;
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
            Location = source.Location,
            ServiceType = source.ServiceType,
            Note = source.Note,
            CreatedAt = source.CreatedAt.ToString(),
            ModifiedAt = source.ModifiedAt.ToString(),
        };
}
