using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Subscriptions.Command.Webhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KlarnaController : ControllerBase
{
    private readonly IMediator _mediator;

    public KlarnaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("session")]
    public async Task<IActionResult> CreateSession([FromBody] CreateKlarnaSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}