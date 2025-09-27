using DroneService.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DroneService.Application.Contracts.Reservations;

public class CreateReservationModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Příspěvek musí mít nějaký text!")]
    [MaxLength(Metadata.ContentLenght)]
    public DateTime ScheduledAt { get; set; }
    public string Location { get; set; } = null!;
    public string ServiceType { get; set; } = null!;
    public string Note { get; set; } = null!;
}
