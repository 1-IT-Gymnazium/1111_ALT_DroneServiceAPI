
using DroneService.Application.Contracts.Payments;

namespace DroneService.Application.Contracts.Interfaces;

public interface ISubscriptionService
{
    Task<PaymentResult> ActivateSubscriptionAsync(PaymentRequest request);
}
