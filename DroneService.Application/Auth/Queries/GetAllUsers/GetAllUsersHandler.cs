using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DroneService.Application.Auth.Queries.GetAllUsers;

// Query handler → vrací seznam uživatelů pro admina
public class GetAllUsersHandler
    : IRequestHandler<GetAllUsersQuery, List<AdminUserListDto>>
{
    private readonly UserManager<AppUser> _userManager;

    public GetAllUsersHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AdminUserListDto>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // ZÁKLADNÍ QUERY (DB)
        // =========================================
        // IQueryable → dotaz se skládá postupně a spustí se až na konci
        var query = _userManager.Users
            .Include(u => u.ServiceGoals)
            .AsQueryable();

        // =========================================
        // FILTRY (stále DB level → efektivní)
        // =========================================

        // Hledání podle jména nebo emailu
        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            query = query.Where(u =>
                u.DisplayName!.Contains(request.DisplayName) ||
                u.Email!.Contains(request.DisplayName));
        }

        // Filtr podle agentury
        if (!string.IsNullOrWhiteSpace(request.AgencyName))
        {
            query = query.Where(u =>
                u.AgencyName!.Contains(request.AgencyName));
        }

        // Filtr podle adresy
        if (!string.IsNullOrWhiteSpace(request.AgencyAddress))
        {
            query = query.Where(u =>
                u.AgencyAddress!.Contains(request.AgencyAddress));
        }

        // Filtr podle kontaktní osoby
        if (!string.IsNullOrWhiteSpace(request.ContactPerson))
        {
            query = query.Where(u =>
                u.ContactPerson!.Contains(request.ContactPerson));
        }

        // =========================================
        // ŘAZENÍ (pořád DB)
        // =========================================
        query = request.Sort == "oldest"
            ? query.OrderBy(u => u.CreatedAt)
            : query.OrderByDescending(u => u.CreatedAt);

        // =========================================
        // SPUŠTĚNÍ DOTAZU (DB → MEMORY)
        // =========================================
        var users = await query.ToListAsync(cancellationToken);

        // =========================================
        // MAPOVÁNÍ + ROLE (už v paměti)
        // =========================================
        var result = new List<AdminUserListDto>();

        foreach (var user in users)
        {
            // ⚠️ KAŽDÝ USER → EXTRA DOTAZ DO DB
            var claims = await _userManager.GetClaimsAsync(user);

            // získání role z claims
            var role = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)
                ?.Value ?? "user";

            // =========================================
            // ROLE FILTER (už v paměti)
            // =========================================
            if (!string.IsNullOrWhiteSpace(request.Role) &&
                role != request.Role)
                continue;

            // mapování na DTO
            result.Add(new AdminUserListDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                AgencyName = user.AgencyName,
                AgencyAddress = user.AgencyAddress,
                ContactPerson = user.ContactPerson,
                Role = role,
                CreatedAt = user.CreatedAt,
                ServiceGoals = user.ServiceGoals?.Select(g => g.Goal).ToList() ?? new List<string>()
            });
        }

        return result;
    }
}