using DroneService.Application.Contracts.Helpers;
using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Payments;
using DroneService.Data;

namespace DroneService.Application.Contracts.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _dbContext;
    private readonly KlarnaService _klarna;

    public SubscriptionService(AppDbContext dbContext, KlarnaService klarna)
    {
        _dbContext = dbContext;
        _klarna = klarna;
    }

    public async Task<PaymentResult> ActivateSubscriptionAsync(PaymentRequest request)
    {
        var price = PaymentHelpers.CalculatePrice(request.ServiceType);

        if (request.ServiceType == "onetime")
        {
            return await _klarna.CreateOneTimeSessionAsync(request, price); 
        }

        var paymentResult = await _klarna.CreateSubscriptionSessionAsync(request, price);
        await _klarna.SavePendingSubscription(request, price);

        return paymentResult; 
    }

}
