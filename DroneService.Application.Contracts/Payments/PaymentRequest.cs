using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DroneService.Application.Contracts.Enums.PaymentEnums;

namespace DroneService.Application.Contracts.Payments;

public class PaymentRequest
{
    public Guid UserId { get; set; }
    public string ServiceType { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; }
    public bool IsSubscription { get; set; }
}

