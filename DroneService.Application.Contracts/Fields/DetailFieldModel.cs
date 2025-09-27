using DroneService.Application.Contracts.Utils;
using DroneService.Data.Entities;

namespace DroneService.Application.Contracts.Fields;

public class DetailFieldModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double Area { get; set; }
    public string CurrentCrops { get; set; } = null!;
    public int ArcGisId { get; set; }
    public int AtticBlock { get; set; }
    public string BlockType { get; set; } = null!;
    public string Municipality { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public string ModifiedAt { get; set; } = null!;
}

public static class DetailFieldModelExtensions
{
    public static DetailFieldModel ToDetailField(this IApplicationMapper mapper, Field source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Area = source.Area,
            CurrentCrops = source.CurrentCrops,
            CreatedAt = source.CreatedAt.ToString(),
            ModifiedAt = source.ModifiedAt.ToString(),
        };
}