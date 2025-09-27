using DroneService.Application.Contracts.Payments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Subscriptions.Command;

public class CreateSubscriptionCommand : IRequest<PaymentResult>
{
    public Guid UserId { get; set; }
    public string ServiceType { get; set; } = null!;
}

