using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Fields;

public class ArcGisResponse
{
    public List<ArcGisFeature> Features { get; set; } = new();
}

public class ArcGisFeature
{
    public ArcGisAttributes Attributes { get; set; } = null!;
}

public class ArcGisAttributes
{
    public double? VYMERA { get; set; }
    public string? ZKOD_DPB { get; set; }
    public string? KULTURANAZ { get; set; }
    public string? PRISL_OPZL { get; set; }
}

