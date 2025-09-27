using DroneService.Application.Reservations.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DroneService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var reservationId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetReservation), new { id = reservationId }, reservationId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(Guid id)
    {
        // Optional: implement GetReservationQuery if needed
        return Ok(new { Message = "Reservation lookup coming soon!", Id = id });
    }
}