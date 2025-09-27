using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Commands.AssignRoleHandler;

public class AssignRoleToUserCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = null!;
}

