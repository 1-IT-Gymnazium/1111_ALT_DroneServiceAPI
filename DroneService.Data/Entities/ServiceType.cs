using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace DroneService.Data.Entities;

public class ServiceType : ITrackable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsSubscription { get; set; }
    public int Price { get; set; }
    public AppUser Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
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
