using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Utilities.Options;

public class ComgateSettings
{
    public Guid MerchantId { get; set; }
    public string Secret { get; set; } = null!;
    public string ApiUrl { get; set; } = null!;
}
