using DroneService.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace DroneService.Data.Entities.Identity;

public class AppUser : IdentityUser<Guid>, ITrackable
{
    public string DisplayName { get; set; } = null!;
    public string? AgencyName { get; set; }
    public ICollection<Field> Fields { get; set; } = [];
    public string? AgencyAddress { get; set; } 
    public string? ContactPerson { get; set; }
    public string? Ico { get; set; }
    public ICollection<ServiceGoal> ServiceGoals { get; set; } = new List<ServiceGoal>();

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
