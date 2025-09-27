using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Enums;

public class PaymentEnums
{
    public enum PaymentMethod
    {
        Card,
        BankTransfer,
        PayPal
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }
    public enum PaymentType
    {
        Subscription,
        OneTime
    }

    public enum SubscriptionType
    {
        Basic,
        Premium,
        Enterprise
    }
}
