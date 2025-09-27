using DroneService.Application.Contracts.Helpers;
using DroneService.Application.Contracts.Payments;
using DroneService.Application.Contracts.Services;
using DroneService.Data;
using DroneService.Data.Entities;
using MediatR;
using NodaTime;

namespace DroneService.Application.Subscriptions.Command;

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, PaymentResult>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly KlarnaService _klarna;

    public CreateSubscriptionCommandHandler(AppDbContext dbContext, IClock clock, KlarnaService klarna)
    {
        _dbContext = dbContext;
        _clock = clock;
        _klarna = klarna;
    }

    public async Task<PaymentResult> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var price = PaymentHelpers.CalculatePrice(request.ServiceType);

        var paymentRequest = new PaymentRequest
        {
            ServiceType = request.ServiceType,
            UserId = request.UserId
            // Add more fields if your KlarnaService expects them
        };

        var klarnaPaymentResult = await _klarna.CreateSubscriptionSessionAsync(paymentRequest, price);
        var clientToken = klarnaPaymentResult.ClientToken;

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            AuthorId = request.UserId,
            ServiceType = request.ServiceType,
            Price = (int)price,
            Status = false,
            CreatedAt = _clock.GetCurrentInstant(),
            CreatedBy = request.UserId.ToString(),
            ModifiedAt = _clock.GetCurrentInstant(),
            ModifiedBy = request.UserId.ToString()
        };

        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PaymentResult
        {
            Success = true,
            Provider = "Klarna",
            ClientToken = clientToken,
            TransactionId = subscription.Id.ToString(),
            Message = "Subscription created",
            Amount = price,
            PaymentType = "subscription"
        };
    }

}

