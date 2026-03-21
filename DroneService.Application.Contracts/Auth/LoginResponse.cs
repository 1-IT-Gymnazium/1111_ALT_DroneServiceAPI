namespace DroneService.Application.Contracts.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public bool RequiresProfileCompletion { get; set; }
}
