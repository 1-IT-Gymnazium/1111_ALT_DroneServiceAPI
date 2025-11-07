using DroneService.Application.Auth.Commands.Admin;
using DroneService.Application.Auth.Commands.AssignRoleHandler;
using DroneService.Application.Auth.Commands.Login;
using DroneService.Application.Auth.Commands.Logout;
using DroneService.Application.Auth.Commands.Refresh;
using DroneService.Application.Auth.Commands.Register;
using DroneService.Application.Auth.Commands.ValidateToken;
using DroneService.Application.Auth.Queries.GetAllUsers;
using DroneService.Application.Auth.Queries.GetUserInfo;
using DroneService.Application.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DroneService.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { token });
    }
    [Authorize]
    [HttpPut("form")]
    public async Task<ActionResult<DetailUserModel>> UpdateForm([FromBody] FormCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { token });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
        {
            return NoContent();
        }

        await _mediator.Send(new LogoutCommand
        {
            RefreshToken = refreshToken
        });

        Response.Cookies.Delete("RefreshToken");

        return NoContent();
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            return Unauthorized(new { Message = "Refresh token not found" });

        var requestInfo = $"{Request.HttpContext.Connection.RemoteIpAddress} | {Request.Headers["User-Agent"]}";
        var result = await _mediator.Send(new RefreshTokenCommand
        {
            RefreshToken = refreshToken,
            RequestInfo = requestInfo
        });

        return Ok(result);
    }


    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userInfo = await _mediator.Send(new GetUserInfoQuery { User = User });
        return Ok(userInfo);
    }

    [Authorize]
    [HttpGet("userInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var result = await _mediator.Send(new GetUserByAgencyName(userId));
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateEmailTokenCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
    {
        await _mediator.Send(command);
        return Ok("Role assigned");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = (bool)await _mediator.Send(request: new DeleteUserCommand(id));
        return success ? NoContent() : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<AppUserDto>>> GetAllUsers()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }
}

