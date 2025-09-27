using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Commands.Logout;

public class LogoutCommand : IRequest<Unit> 
{
    public string? RefreshToken { get; set; }
}
