using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Payments;
using DroneService.Application.Subscriptions.Command.Webhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService, IMediator mediator)
    {
        _subscriptionService = subscriptionService;
        _mediator = mediator;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] PaymentRequest request)
    {
        var result = await _subscriptionService.ActivateSubscriptionAsync(request);
        return Ok(result);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] KlarnaWebhookDto data)
    {
        var command = new ProcessKlarnaWebhookCommand
        {
            EventType = data.EventType,
            OrderId = data.OrderId,
            Status = data.Status,
            MerchantReference = data.MerchantReference
        };

        await _mediator.Send(command);
        return Ok();
    }

}
