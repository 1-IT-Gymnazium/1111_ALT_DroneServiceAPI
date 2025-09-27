using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string username, string email, int expiresInMinutes, IList<Claim> claims);
    Task<string> GenerateRefreshTokenAsync(Guid userId, int expirationInDays, string requestInfo, IList<Claim> claims);
    string Hash(string input);
}
