using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Services;
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

public class FormCommandHandler : IRequestHandler<FormCommand, DetailUserModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFieldImportService _fieldService;

    public FormCommandHandler(
        AppDbContext dbContext,
        IClock clock,
        IApplicationMapper mapper,
        UserManager<AppUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IFieldImportService fieldService)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _fieldService = fieldService;
    }

    public async Task<DetailUserModel> Handle(FormCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User?
    .FindFirstValue(ClaimTypes.NameIdentifier);

if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
             throw new Exception("USER_NOT_FOUND");

        var user = await _userManager.Users
            .Include(u => u.ServiceGoals)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new Exception("USER_NOT_FOUND");

        user.AgencyName = request.AgencyName;
        user.ContactPerson = request.ContactPerson;
        user.AgencyAddress = request.AgencyAddress;
        user.Ico = request.Ico;
        user.SetModifyBy("System", _clock.GetCurrentInstant());

        var incoming = (request.ServiceGoals ?? new List<string>())
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Select(g => g.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existing = await _dbContext.ServiceGoals
            .Where(g => g.UserId == user.Id)
            .ToListAsync(cancellationToken);

        var toRemove = existing
            .Where(e => !incoming.Contains(e.Goal, StringComparer.OrdinalIgnoreCase))
            .ToList();
        _dbContext.ServiceGoals.RemoveRange(toRemove);

        var existingSet = new HashSet<string>(existing.Select(e => e.Goal), StringComparer.OrdinalIgnoreCase);
        var toAdd = incoming
            .Where(g => !existingSet.Contains(g))
            .Select(g => new ServiceGoal { Id = Guid.NewGuid(), Goal = g, UserId = user.Id });

        await _dbContext.ServiceGoals.AddRangeAsync(toAdd, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        

        var updatedUser = await _userManager.Users
            .Include(u => u.ServiceGoals)
            .FirstAsync(u => u.Id == user.Id, cancellationToken);

        return _mapper.ToDetailUser(updatedUser);
    }
}
