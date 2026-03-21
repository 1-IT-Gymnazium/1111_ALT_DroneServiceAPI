using DroneService.Data.Entities;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace DroneService.Application.Contracts.Reservations;

public class CreateReservationModel
{
    [Required]
    public Instant ScheduledAt { get; set; }
    [Required]
    [MinLength(1)]
    public List<Guid> FieldIds { get; set; } = null!;
}
