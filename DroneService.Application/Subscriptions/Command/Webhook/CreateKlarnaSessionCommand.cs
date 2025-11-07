using DroneService.Application.Contracts.Klarna;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Subscriptions.Command.Webhook;

public record CreateKlarnaSessionCommand(Guid SubscriptionId) : IRequest<PaymentResult>;