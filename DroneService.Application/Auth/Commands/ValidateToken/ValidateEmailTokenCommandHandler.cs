using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Auth.Commands.ValidateToken;

public class ValidateEmailTokenCommandHandler : IRequestHandler<ValidateEmailTokenCommand, IResult>
{
    private readonly UserManager<AppUser> _userManager;

    public ValidateEmailTokenCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IResult> Handle(ValidateEmailTokenCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToUpperInvariant();
        var user = await _userManager.Users
            .SingleOrDefaultAsync(x => !x.EmailConfirmed && x.NormalizedUserName == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { nameof(request.Token), new[] { "INVALID_TOKEN" } }
            });
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { nameof(request.Token), new[] { "INVALID_TOKEN" } }
            });
        }

        return Results.NoContent();
    }
}

