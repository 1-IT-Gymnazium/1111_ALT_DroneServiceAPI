using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DroneService.Data.Entities;
[Table(nameof(Field))]
public class Field : ITrackable
{
    public Guid Id { get; set; }
    [MaxLength(Metadata.MaxNameLength)]
    public string? Name { get; set; }
    [MaxLength(Metadata.ContentLenght)]
    public double Area { get; set; } 
    [MaxLength(Metadata.ContentLenght)]
    public string? CurrentCrops { get; set; }
    public int AtticBlock { get; set; }
    public string BlockType { get; set; } = null!;
    public string Municipality { get; set; } = null!;

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

    public static class Metadata
    {
        public const int ContentLenght = DatabaseConstants.ContentLengh;
        public const int TrackableByLength = DatabaseConstants.TrackableLength;
        public const int MaxNameLength = DatabaseConstants.MaxNameLength;
        public const int ShortMaxLength = DatabaseConstants.ShortMaxLength;
    }
}
