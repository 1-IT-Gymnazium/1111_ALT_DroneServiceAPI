using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Payments;

public class KlarnaWebhookData
{
    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = null!;

    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = null!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("merchant_reference1")]
    public string MerchantReference { get; set; } = null!;
}

