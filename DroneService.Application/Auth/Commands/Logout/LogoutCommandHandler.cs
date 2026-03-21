using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Auth.Commands.Logout;

// Handler pro logout → nevrací žádná data (Unit = void)
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly AppDbContext _dbContext;

    // IClock (NodaTime) → lepší než DateTime.Now (testovatelný čas)
    private readonly IClock _clock;

    public LogoutCommandHandler(AppDbContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Pokud není refresh token → nic neděláme
        if (string.IsNullOrEmpty(request.RefreshToken))
            return Unit.Value;

        // Token neukládáš v DB v plain textu → správně!
        // Hashujeme ho, abychom ho mohli porovnat
        var hashedToken = Hash(request.RefreshToken);

        // Najdeme token v databázi
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == hashedToken, cancellationToken);

        // Pokud:
        // - token neexistuje
        // - nebo už expiroval
        // - nebo už byl revoke-nutý
        // → nic neděláme (bezpečné chování)
        if (storedToken is null
            || storedToken.ExpiresAt < _clock.GetCurrentInstant()
            || storedToken.RevokedAt != null)
            return Unit.Value;

        // "Logout" = zneplatnění refresh tokenu
        // → nastavíme expiraci na teď
        storedToken.ExpiresAt = _clock.GetCurrentInstant();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Hashovací metoda → SHA256
    private string Hash(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();

        var bytes = System.Text.Encoding.UTF8.GetBytes(token);

        var hash = sha.ComputeHash(bytes);

        // uložíme jako Base64 string (v DB)
        return Convert.ToBase64String(hash);
    }
}