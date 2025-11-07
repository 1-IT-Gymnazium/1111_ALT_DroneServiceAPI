using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Services;
using DroneService.Application.Contracts.ServiceType;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Fields.Commands;
using DroneService.Application.ServiceTypes.Command;
using DroneService.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace DroneService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServiceTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceModel model)
    {
        var command = new CreateServiceCommand(model, User.GetUserId());
        var result = await _mediator.Send(command) as DetailServiceModel;
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpGet]
    public IActionResult GetAll([FromServices] AppDbContext db)
    {
        var services = db.ServiceType.Select(s => new DetailServiceModel
        {
            Id = s.Id,
            Name = s.Name,
            IsSubscription = s.IsSubscription,
        }).ToList();

        return Ok(services);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DetailServiceModel model, [FromServices] AppDbContext db)
    {
        var service = await db.ServiceType.FindAsync(id);
        if (service == null) return NotFound();

        service.Name = model.Name;
        service.IsSubscription = model.IsSubscription;

        await db.SaveChangesAsync();
        return Ok(model);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] AppDbContext db)
    {
        var service = await db.ServiceType.FindAsync(id);
        if (service == null) return NotFound();

        db.ServiceType.Remove(service);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
