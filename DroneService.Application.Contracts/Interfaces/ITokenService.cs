using System.Security.Claims;

namespace DroneService.Application.Contracts.Interfaces;

// Interface pro práci s JWT tokeny (access + refresh)
// Definuje metody pro generování a zpracování tokenů
public interface ITokenService
{
    // Vytvoří access token (JWT)
    // obsahuje základní informace o uživateli + claims (např. role)
    string GenerateAccessToken(
        Guid userId,          // ID uživatele
        string username,      // uživatelské jméno
        string email,         // email uživatele
        int expiresInMinutes, // doba platnosti tokenu
        IList<Claim> claims   // dodatečné claims (např. role)
    );

    // Vytvoří refresh token (dlouhodobý token pro obnovu přihlášení)
    Task<string> GenerateRefreshTokenAsync(
        Guid userId,           // ID uživatele
        int expirationInDays,  // doba platnosti refresh tokenu
        string requestInfo,    // informace o zařízení / requestu (např. IP, user-agent)
        IList<Claim> claims    // případné claims
    );

    // Hashovací funkce pro tokeny (např. pro bezpečné uložení do DB)
    string Hash(string input);
}