using NodaTime;

namespace DroneService.Application.Contracts.Auth;

public class AdminUserListDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AgencyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string AgencyAddress { get; set; } = string.Empty;
    public string Ico { get; set; } = string.Empty;
    public List<string> ServiceGoals { get; set; } = new();
    public Instant CreatedAt { get; set; }
}
