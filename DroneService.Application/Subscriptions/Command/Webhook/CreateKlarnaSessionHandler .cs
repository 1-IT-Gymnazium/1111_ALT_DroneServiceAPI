using DroneService.Application.Contracts.Klarna;
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

public class CreateKlarnaSessionHandler : IRequestHandler<CreateKlarnaSessionCommand, PaymentResult>
{
    public Task<PaymentResult> Handle(CreateKlarnaSessionCommand request, CancellationToken cancellationToken)
    {
        // 🔹 Později sem doplníš reálný KlarnaService.CreateSession(...)
        return Task.FromResult(new PaymentResult
        {
            Success = true,
            Provider = "Klarna",
            Message = "Simulated Klarna session (for now)"
        });
    }
}

