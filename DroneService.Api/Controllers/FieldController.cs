using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Fields.Commands;
using DroneService.Application.Fields.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DroneService.Api.Controllers;

[ApiController]
[Route("api/field")]
public class FieldController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<DetailFieldModel>> Create([FromBody] CreateFieldModel model)
    {
        var command = new CreateFieldCommand(model, User.GetUserId());
        var result = await _mediator.Send(command) as DetailFieldModel;
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPost("from-lpis")]
    public async Task<ActionResult<DetailFieldModel>> CreateFromLpis([FromBody] CreateFieldFromLpisCommand command)
    {
        var authorIdClaim = User.FindFirst("sub")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (authorIdClaim == null)
            return Unauthorized();

        command.AuthorId = Guid.Parse(authorIdClaim);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DetailFieldModel>> Update(Guid id, [FromBody] CreateFieldModel model)
    {
        var result = await _mediator.Send(new UpdateFieldCommand(id, model));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DetailFieldModel>> Get(Guid id)
    {
        var result = await _mediator.Send(new GetFieldByIdQuery(id));
        return result != null ? Ok(result) : NotFound();
    }

    //all fiedls query 
    [HttpGet("list")]
    public async Task<ActionResult<List<DetailFieldModel>>> GetList()
    {
        return Ok(await _mediator.Send(new GetAllFieldsQuery()));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = (bool)await _mediator.Send(request: new DeleteFieldCommand(id));
        return success ? NoContent() : NotFound();
    }
    [Authorize]
    [HttpGet("my-fields")]
    public async Task<IActionResult> GetMyFields()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetUserFieldsQuery(userId));
        return Ok(result);
    }
}

