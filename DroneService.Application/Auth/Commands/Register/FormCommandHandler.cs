using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Result;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Security.Claims;

namespace DroneService.Application.Auth.Commands.Register;

// Handler → doplnění profilu uživatele po registraci
// (např. agentura, kontaktní osoba, cíle služeb atd.)
public class FormCommandHandler : IRequestHandler<FormCommand, Result<DetailUserModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FormCommandHandler(
        AppDbContext dbContext,
        IClock clock,
        IApplicationMapper mapper,
        UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<DetailUserModel>> Handle(FormCommand request, CancellationToken cancellationToken)
    {
        // =========================================
        // 1. ZÍSKÁNÍ USER ID Z JWT
        // =========================================
        var userIdStr = _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        // kontrola validity ID
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Result<DetailUserModel>.Fail("USER_NOT_FOUND");

        // =========================================
        // 2. NAČTENÍ UŽIVATELE + JEHO SERVICE GOALS
        // =========================================
        var user = await _userManager.Users
            .Include(u => u.ServiceGoals) // eager loading → potřebujeme vztah
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return Result<DetailUserModel>.Fail("USER_NOT_FOUND");

        // =========================================
        // 3. UPDATE ZÁKLADNÍCH ÚDAJŮ
        // =========================================
        user.AgencyName = request.AgencyName;
        user.ContactPerson = request.ContactPerson;
        user.AgencyAddress = request.AgencyAddress;
        user.Ico = request.Ico;
        user.Note = request.Note;

        // audit (kdo a kdy upravil)
        user.SetModifyBy("System", _clock.GetCurrentInstant());

        // =========================================
        // 4. ZPRACOVÁNÍ SERVICE GOALS (NEJSLOŽITĚJŠÍ ČÁST 🔥)
        // =========================================

        // 4.1 Normalizace vstupu (očistíme data)
        var incoming = (request.ServiceGoals ?? new List<string>())
            .Where(g => !string.IsNullOrWhiteSpace(g)) // odstraníme prázdné
            .Select(g => g.Trim())                     // odstraníme mezery
            .Distinct(StringComparer.OrdinalIgnoreCase) // odstraníme duplicity
            .ToList();

        // 4.2 Načteme existující cíle z DB
        var existing = await _dbContext.ServiceGoals
            .Where(g => g.UserId == user.Id)
            .ToListAsync(cancellationToken);

        // 4.3 Najdeme co smazat
        var toRemove = existing
            .Where(e => !incoming.Contains(e.Goal, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // smažeme z DB
        _dbContext.ServiceGoals.RemoveRange(toRemove);

        // 4.4 Připravíme set pro rychlé porovnání
        var existingSet = new HashSet<string>(
            existing.Select(e => e.Goal),
            StringComparer.OrdinalIgnoreCase);

        // 4.5 Najdeme co přidat
        var toAdd = incoming
            .Where(g => !existingSet.Contains(g))
            .Select(g => new ServiceGoal
            {
                Id = Guid.NewGuid(),
                Goal = g,
                UserId = user.Id
            });

        // přidáme nové
        await _dbContext.ServiceGoals.AddRangeAsync(toAdd, cancellationToken);

        // =========================================
        // 5. ULOŽENÍ ZMĚN
        // =========================================
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 6. NAČTENÍ AKTUALIZOVANÉHO USERA
        // =========================================
        var updatedUser = await _userManager.Users
            .Include(u => u.ServiceGoals)
            .FirstAsync(u => u.Id == user.Id, cancellationToken);

        // =========================================
        // 7. MAPOVÁNÍ NA DTO
        // =========================================
        return Result<DetailUserModel>.Ok(
            _mapper.ToDetailUser(updatedUser)
        );
    }
}