using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Data.Entities;
[Table(nameof(Subscription))]
public class Subscription : ITrackable
{
    public Guid Id { get; set; }
    public string ServiceType { get; set; } = null!;
    public int Price { get; set; }
    public bool Status { get; set; }
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
