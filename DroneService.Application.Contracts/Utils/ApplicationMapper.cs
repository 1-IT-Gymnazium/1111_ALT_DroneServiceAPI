using DroneService.Application.Contracts.Auth;
using DroneService.Data;
using DroneService.Data.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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