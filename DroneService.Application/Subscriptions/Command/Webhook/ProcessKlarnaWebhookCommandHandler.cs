using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Subscriptions.Command.Webhook;

public class ProcessKlarnaWebhookCommandHandler : IRequestHandler<ProcessKlarnaWebhookCommand, Unit>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;

    public ProcessKlarnaWebhookCommandHandler(AppDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Unit> Handle(ProcessKlarnaWebhookCommand request, CancellationToken cancellationToken)
    {
        if (request.EventType != "ORDER_PLACED" || request.Status != "AUTHORIZED")
            return Unit.Value;

        var userId = Guid.Parse(request.MerchantReference);

        var subscription = await _dbContext.Subscriptions
            .Where(s => s.AuthorId == userId && !s.Status)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription != null)
        {
            subscription.Status = true;
            subscription.ModifiedAt = _clock.GetCurrentInstant();
            subscription.ModifiedBy = "KlarnaWebhook";

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}

