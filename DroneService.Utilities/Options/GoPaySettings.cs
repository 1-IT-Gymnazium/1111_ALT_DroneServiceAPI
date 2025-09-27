using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Utilities.Options;

public class GoPaySettings
{
    public Guid ClientId { get; set; }
    public string ClientSecret { get; set; } = null!;
    public string ApiUrl { get; set; } = null!;
}
