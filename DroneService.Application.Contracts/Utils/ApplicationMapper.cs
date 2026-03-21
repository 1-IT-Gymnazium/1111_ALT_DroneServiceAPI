using DroneService.Application.Contracts.Auth;
using DroneService.Data.Entities.Identity;
using NodaTime;

namespace DroneService.Application.Contracts.Utils;

public class ApplicationMapper : IApplicationMapper
{
    private readonly IClock _clock;
    public ApplicationMapper(IClock clock) => _clock = clock;

    public Instant Now => _clock.GetCurrentInstant();

    public DetailUserModel ToDetailUser(AppUser user)
    {
        return new DetailUserModel
        {
            Id = user.Id,
            DisplayName = user.DisplayName ?? string.Empty,
            AgencyName = user.AgencyName ?? string.Empty,
            ContactPerson = user.ContactPerson ?? string.Empty,
            AgencyAddress = user.AgencyAddress ?? string.Empty,
            Ico = user.Ico ?? string.Empty,
            ServiceGoals = user.ServiceGoals?.Select(g => g.Goal).ToList() ?? new List<string>()
        };
    }
}