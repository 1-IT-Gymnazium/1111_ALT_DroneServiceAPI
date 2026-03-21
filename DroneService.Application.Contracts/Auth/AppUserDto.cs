namespace DroneService.Application.Contracts.Auth;

public class AppUserDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
