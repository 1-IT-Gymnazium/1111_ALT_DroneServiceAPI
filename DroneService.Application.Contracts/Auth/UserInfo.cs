using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Auth;

public class UserInfo
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string AgencyName { get; set; } = null!;
}
