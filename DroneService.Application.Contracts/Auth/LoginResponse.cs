using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public bool RequiresProfileCompletion { get; set; }
}
