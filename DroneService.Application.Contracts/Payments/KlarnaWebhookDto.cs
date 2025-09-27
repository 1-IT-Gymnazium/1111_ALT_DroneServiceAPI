using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Payments;

public class KlarnaWebhookDto
{
    public string EventType { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string MerchantReference { get; set; } = null!;
}

