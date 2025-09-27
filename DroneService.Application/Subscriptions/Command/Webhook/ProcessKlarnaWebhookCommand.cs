using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Subscriptions.Command.Webhook;

public class ProcessKlarnaWebhookCommand : IRequest<Unit>
{
    public string EventType { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string MerchantReference { get; set; } = null!;
}

