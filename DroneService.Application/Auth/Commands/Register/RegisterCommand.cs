using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Commands.Register;

public class RegisterCommand : IRequest<string>
{
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
