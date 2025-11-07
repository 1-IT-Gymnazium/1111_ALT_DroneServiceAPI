using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DroneService.Data.Entities;
[Table(nameof(Reservation))]

public class Reservation : ITrackable
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string ServiceType { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsSubscription { get; set; }
    public AppUser Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public ICollection<Field> Fields { get; set; } = new List<Field>();

    #region Trackable
    public Instant CreatedAt { get; set; }
    [MaxLength(Metadata.TrackableByLength)]
    public string CreatedBy { get; set; } = null!;
    [MaxLength(Metadata.TrackableByLength)]
    public Instant ModifiedAt { get; set; }
    [MaxLength(Metadata.TrackableByLength)]
    public string ModifiedBy { get; set; } = null!;
    public Instant? DeletedAt { get; set; }
    [MaxLength(Metadata.TrackableByLength)]
    public string? DeletedBy { get; set; }
    #endregion
}
public static class Metadata
{
    public const int ContentLenght = DatabaseConstants.ContentLengh;
    public const int TrackableByLength = DatabaseConstants.TrackableLength;
    public const int MaxNameLength = DatabaseConstants.MaxNameLength;
    public const int ShortMaxLength = DatabaseConstants.ShortMaxLength;
}