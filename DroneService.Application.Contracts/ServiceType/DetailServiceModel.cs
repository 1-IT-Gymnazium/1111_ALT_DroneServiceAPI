using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.ServiceType;

public class DetailServiceModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsSubscription { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string ModifiedAt { get; set; } = null!;
}
public static class DetailServiceModelExtensions
{
    public static DetailServiceModel ToDetailField(this IApplicationMapper mapper, DroneService.Data.Entities.ServiceType source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            IsSubscription = source.IsSubscription,
            CreatedAt = source.CreatedAt.ToString(),
            ModifiedAt = source.ModifiedAt.ToString(),
        };
}