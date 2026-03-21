using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Fields.Commands.CreateField;
using DroneService.Application.Fields.Commands.CreateFieldFromLpis;
using DroneService.Application.Fields.Commands.DeleteField;
using DroneService.Application.Fields.Commands.UpdateField;
using DroneService.Application.Fields.Queries.GetAllFields;
using DroneService.Application.Fields.Queries.GetUsersFields;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DroneService.Api.Controllers;

// API controller → automatický binding, validace, atd.
[ApiController]

// URL: /api/Field
[Route("api/[controller]")]
public class FieldController(IMediator mediator) : ControllerBase
{
    // Primary constructor (novější syntaxe C#)
    // mediator se rovnou uloží do fieldu
    private readonly IMediator _mediator = mediator;

    // =========================================
    // CREATE FIELD (ruční vytvoření)
    // =========================================

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DetailFieldModel>> Create([FromBody] CreateFieldModel model)
    {
        // Vytvoření commandu + přidání ID uživatele z JWT
        var command = new CreateFieldCommand(model, User.GetUserId());

        // Mediator → pošle command do handleru
        var result = await _mediator.Send(command) as DetailFieldModel;

        // Vrací HTTP 201 + odkaz na GET endpoint
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    // =========================================
    // CREATE FIELD FROM LPIS (externí data)
    // =========================================

    [Authorize]
    [HttpPost("from-lpis")]
    public async Task<ActionResult<DetailFieldModel>> CreateFromLpis([FromBody] CreateFieldFromLpisCommand command)
    {
        // Snažíme se získat userId z různých typů claimů
        // (někdy je "sub", jindy NameIdentifier → záleží na JWT konfiguraci)
        var authorIdClaim = User.FindFirst("sub")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (authorIdClaim == null)
            return Unauthorized();

        // Přiřazení autora do commandu
        command.AuthorId = Guid.Parse(authorIdClaim);

        // Zavolání handleru → ten pravděpodobně:
        // - stáhne data z LPIS / ArcGIS
        // - vytvoří field v DB
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    // =========================================
    // UPDATE FIELD
    // =========================================

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DetailFieldModel>> Update(Guid id, [FromBody] CreateFieldModel model)
    {
        // Command pro update
        var result = await _mediator.Send(new UpdateFieldCommand(id, model));

        // null → field neexistuje
        return result != null ? Ok(result) : NotFound();
    }

    // =========================================
    // GET FIELD BY ID
    // =========================================

    // GET /api/Field/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DetailFieldModel>> Get(Guid id)
    {
        // Query → načte field podle ID
        var result = await _mediator.Send(new GetFieldByIdQuery(id));

        return result != null ? Ok(result) : NotFound();
    }

    // =========================================
    // GET ALL FIELDS (ADMIN ONLY)
    // =========================================

    // GET /api/Field/list
    [Authorize(Roles = "Admin")]
    [HttpGet("list")]
    public async Task<ActionResult<List<DetailFieldModel>>> GetList([FromQuery] GetAllFieldsQuery query)
    {
        // Query se binduje přímo z query stringu
        return Ok(await _mediator.Send(query));
    }

    // =========================================
    // DELETE FIELD
    // =========================================

    // ⚠️ POZOR – tady chybí [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // Command vrací bool → jestli se smazání povedlo
        var success = (bool)await _mediator.Send(request: new DeleteFieldCommand(id));

        return success ? NoContent() : NotFound();
    }

    // =========================================
    // GET MY FIELDS
    // =========================================

    [Authorize]
    [HttpGet("my-fields")]
    public async Task<IActionResult> GetMyFields()
    {
        // Získání userId z JWT
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Validace GUID (bezpečnější než Parse)
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        // Query → vrátí fieldy daného uživatele
        var result = await _mediator.Send(new GetUserFieldsQuery(userId));

        return Ok(result);
    }
}