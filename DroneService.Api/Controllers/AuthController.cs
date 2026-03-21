using DroneService.Application.Auth.Commands.Admin;
using DroneService.Application.Auth.Commands.AssignRoleHandler;
using DroneService.Application.Auth.Commands.Login;
using DroneService.Application.Auth.Commands.Logout;
using DroneService.Application.Auth.Commands.Refresh;
using DroneService.Application.Auth.Commands.Register;
using DroneService.Application.Auth.Commands.ValidateToken;
using DroneService.Application.Auth.Queries.GetUserInfo;
using DroneService.Application.Auth.Queries.GetAllUsers;
using DroneService.Application.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DroneService.Api.Controllers;

// API controller
[ApiController]

// URL: /api/Auth
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // MediatR → veškerá logika je v handlerech (CQS)
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================
    // REGISTER (registrace uživatele)
    // =========================================

    // POST /api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        // Handler:
        // - vytvoří uživatele
        // - uloží do DB
        // - vygeneruje JWT token
        var token = await _mediator.Send(command);

        // Vrací token (frontend si ho uloží)
        return Ok(new { token });
    }

    // =========================================
    // UPDATE USER FORM (např. profil)
    // =========================================

    [Authorize]
    [HttpPut("form")]
    public async Task<ActionResult<DetailUserModel>> UpdateForm([FromBody] FormCommand command, CancellationToken ct)
    {
        // Command → update uživatelských dat
        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    // =========================================
    // LOGIN
    // =========================================

    // POST /api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        // Handler:
        // - ověří email + heslo
        // - vygeneruje JWT
        // - pravděpodobně nastaví refresh token cookie
        var token = await _mediator.Send(command);

        return Ok(new { token });
    }

    // =========================================
    // LOGOUT
    // =========================================

    // POST /api/Auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Zkusíme vytáhnout refresh token z cookie
        if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
        {
            // Pokud neexistuje → nic neděláme
            return NoContent();
        }

        // Pošleme command → invaliduje refresh token v DB
        await _mediator.Send(new LogoutCommand
        {
            RefreshToken = refreshToken
        });

        // Smažeme cookie na klientovi
        Response.Cookies.Delete("RefreshToken");

        return NoContent();
    }

    // =========================================
    // REFRESH TOKEN (velmi důležité)
    // =========================================

    // POST /api/Auth/Refresh
    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        // Vytáhneme refresh token z cookie
        if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            return Unauthorized(new { Message = "Refresh token not found" });

        // Info o requestu (bezpečnostní log / audit)
        var requestInfo = $"{Request.HttpContext.Connection.RemoteIpAddress} | {Request.Headers["User-Agent"]}";

        // Handler:
        // - ověří refresh token
        // - vytvoří nový JWT
        var result = await _mediator.Send(new RefreshTokenCommand
        {
            RefreshToken = refreshToken,
            RequestInfo = requestInfo
        });

        return Ok(result);
    }

    // =========================================
    // GET CURRENT USER (z JWT)
    // =========================================

    // GET /api/Auth/me
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // Pošleme celý ClaimsPrincipal do query
        var userInfo = await _mediator.Send(new GetUserInfoQuery { User = User });

        return Ok(userInfo);
    }

    // =========================================
    // GET USER INFO (jiný endpoint)
    // =========================================

    [Authorize]
    [HttpGet("userInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        // Získání userId z JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        // Query podle userId
        var result = await _mediator.Send(new GetUserByAgencyName(userId));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // =========================================
    // VALIDATE EMAIL TOKEN
    // =========================================

    // POST /api/Auth/validateToken
    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateEmailTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // =========================================
    // ASSIGN ROLE (⚠️ pozor!)
    // =========================================

    // POST /api/Auth/assign-role
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
    {
        // Přiřazení role uživateli (např. Admin)
        await _mediator.Send(command);

        return Ok("Role assigned");
    }

    // =========================================
    // DELETE USER (ADMIN)
    // =========================================

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = (bool)await _mediator.Send(request: new DeleteUserCommand(id));

        return success ? NoContent() : NotFound();
    }

    // =========================================
    // GET ALL USERS (ADMIN)
    // =========================================

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetUsers([FromQuery] GetAllUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}