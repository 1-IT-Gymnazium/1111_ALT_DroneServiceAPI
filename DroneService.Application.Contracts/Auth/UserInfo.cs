namespace DroneService.Application.Contracts.Auth;

public class UserInfo
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string AgencyName { get; set; } = null!;
    public string AgencyAddress { get; set; } = null!;
}
