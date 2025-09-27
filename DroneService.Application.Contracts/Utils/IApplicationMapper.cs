using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Reservations;
using DroneService.Data.Entities;
using DroneService.Data.Entities.Identity;
using NodaTime;

namespace DroneService.Application.Contracts.Utils;

public interface IApplicationMapper
{
    Instant Now { get; }
    DetailUserModel ToDetailUser(AppUser user);
}
