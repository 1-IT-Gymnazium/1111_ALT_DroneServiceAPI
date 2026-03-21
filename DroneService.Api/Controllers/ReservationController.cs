using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Reservation.Commands.CreateReservation;
using DroneService.Application.Reservation.Commands.DeleteReservation;
using DroneService.Application.Reservation.Commands.UpdateReservation;
using DroneService.Application.Reservation.Commands.UpdateReservationStatus;
using DroneService.Application.Reservation.Queries.GetReservationByDateRange;
using DroneService.Application.Reservation.Queries.GetReservationFiltered;
using DroneService.Application.Reservation.Queries.GetReservationHistory;
using DroneService.Application.Reservation.Queries.GetUserReservation;
using DroneService.Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.Text;
using System.Security.Claims;

namespace DroneService.Api.Controllers;

// API controller → automatická validace, binding, atd.
[ApiController]

// URL: /api/Reservations
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    // MediatR → oddělení logiky (CQS pattern)
    private readonly IMediator _mediator;

    // DI – mediator se injectne automaticky
    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================
    // CREATE RESERVATION
    // =========================================

    // Pouze přihlášený uživatel
    [Authorize]

    // POST /api/Reservations
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        // Command už obsahuje všechna data → rovnou ho pošleme
        var result = await _mediator.Send(command);

        // Pokud command selže → vrátíme chybu
        if (!result.Success)
            return BadRequest(result.Error);

        // Jinak vracíme vytvořenou rezervaci
        return Ok(result.Value);
    }

    // =========================================
    // UPDATE RESERVATION
    // =========================================

    // PUT /api/Reservations/{id}
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DetailReservationModel>> Update(Guid id, [FromBody] CreateReservationModel model)
    {
        // Vytvoříme command ručně z modelu
        var result = await _mediator.Send(new UpdateReservationCommand(
            id,
            model.FieldIds,
            model.ScheduledAt
        ));

        // Pokud nic nevrátil → neexistuje
        return result != null ? Ok(result) : NotFound();
    }

    // =========================================
    // GET MY RESERVATIONS
    // =========================================

    // GET /api/Reservations/my-reservations
    [Authorize]
    [HttpGet("my-reservations")]
    public async Task<IActionResult> GetReservation()
    {
        // Získání userId z JWT tokenu (claim)
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Validace GUID
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        // Query → vrátí rezervace daného uživatele
        var result = await _mediator.Send(new GetUserReservationQuery(userId));

        return Ok(result);
    }

    // =========================================
    // DELETE RESERVATION
    // =========================================

    // DELETE /api/Reservations/{id}
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteReservationCommand(id));

        // false → rezervace neexistuje
        if (!result)
            return NotFound();

        // 204 = úspěšně smazáno
        return NoContent();
    }

    // =========================================
    // UPDATE STATUS (ADMIN ONLY)
    // =========================================

    // PUT /api/Reservations/{id}/status
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<DetailReservationModel>> UpdateStatus(
        Guid id,
        [FromBody] UpdateReservationStatusRequest request)
    {
        var result = await _mediator.Send(
            new UpdateReservationStatusCommand(id, request.Status));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // =========================================
    // GET RESERVATIONS BY DATE RANGE (ADMIN)
    // =========================================

    // GET /api/Reservations/get-reservations?from=2025-01-01&days=7
    [HttpGet("get-reservations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReservations(
        [FromQuery] string from,
        [FromQuery] int days)
    {
        // Parsování stringu na LocalDate (YYYY-MM-DD)
        var localDate = LocalDatePattern.Iso.Parse(from).Value;

        // Převod na Instant (UTC čas od epochy)
        var fromInstant = localDate
            .AtStartOfDayInZone(DateTimeZone.Utc)
            .ToInstant();

        // Query → vrátí rezervace v daném rozsahu
        var result = await _mediator.Send(
            new GetReservationsByDateRangeQuery(fromInstant, days)
        );

        return Ok(result);
    }

    // =========================================
    // FILTERED RESERVATIONS (USER)
    // =========================================

    // GET /api/Reservations/my-reservations/filtered
    [Authorize]
    [HttpGet("my-reservations/filtered")]
    public async Task<IActionResult> GetMyFiltered(
        [FromQuery] Instant? from,
        [FromQuery] Instant? to,
        [FromQuery] ReservationState? state,
        [FromQuery] string? serviceType,
        [FromQuery] Guid? fieldId
    )
    {
        // Získání userId z tokenu
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        // Query s filtry (může být kombinace všeho)
        var result = await _mediator.Send(
            new GetReservationsFilteredQuery(
                from,
                to,
                state,
                serviceType,
                fieldId,
                userId
            )
        );

        return Ok(result);
    }

    // =========================================
    // ALL HISTORY (ADMIN)
    // =========================================

    // GET /api/Reservations/history
    [Authorize(Roles = "Admin")]
    [HttpGet("history")]
    public async Task<IActionResult> GetAllHistory()
    {
        var result = await _mediator.Send(new GetReservationHistoryQuery());
        return Ok(result);
    }

    // =========================================
    // MY HISTORY
    // =========================================

    // GET /api/Reservations/my-history
    [Authorize]
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory()
    {
        // Tady už se nevaliduje → předpokládá se, že claim existuje
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var result = await _mediator.Send(
            new GetReservationHistoryQuery
            {
                // filtr jen na aktuálního uživatele
                AuthorId = userId
            });

        return Ok(result);
    }
}